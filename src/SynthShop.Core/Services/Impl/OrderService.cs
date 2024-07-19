using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Enums;
using SynthShop.Domain.Exceptions;
using SynthShop.Domain.Models;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Core.Services.Impl;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IEmailProducer _emailProducer;
    private readonly IUserProvider _userProvider;

   public OrderService(IOrderRepository orderRepository, IBasketRepository basketRepository, ILogger logger,
            IUnitOfWork unitOfWork, IProductRepository productRepository, IEmailProducer emailProducer,
            IUserProvider userProvider)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _emailProducer = emailProducer;
            _userProvider = userProvider;
        }


    public async Task<Result<Order>> CreateOrder(Guid basketId, Guid customerId)
    {
        try
        {
            var basket = await _basketRepository.GetBasketByIdAsync(basketId);

            if (basket == null)
                return new Result<Order>(new OrderFailedException("Basket not found."));

            basket.CustomerId ??= customerId;

            if (!basket.Items.Any()) return new Result<Order>(new OrderFailedException("Basket is empty."));

            var availabilityIssues = new List<string>();
            foreach (var item in basket.Items)
                if (item.Product.StockQuantity < item.Quantity)
                    availabilityIssues.Add(
                        $"Not enough stock for {item.Product.Name}. Requested: {item.Quantity}, Available: {item.Product.StockQuantity}");
                else
                    item.Product.StockQuantity -= item.Quantity;

            if (availabilityIssues.Any())
            {
                _logger.Warning("Failed to create order due to concurrency conflicts for basket ID {BasketId}",
                    basketId);
                return new Result<Order>(new OrderFailedException(string.Join(", ", availabilityIssues)));
            }

            var order = new Order
            {
                OrderID = Guid.NewGuid(),
                OrderDate = DateTime.UtcNow,
                UserId = customerId,
                Status = OrderStatus.Pending,
                OrderItems = basket.Items.Select(bi => new OrderItem
                {
                    OrderItemID = Guid.NewGuid(),
                    ProductID = bi.ProductId,
                    Quantity = bi.Quantity,
                    Price = bi.Product.Price,
                    CreatedAt = DateTime.UtcNow
                }).ToList(),
                TotalAmount = basket.Items.Sum(i => i.Product.Price * i.Quantity),
                CreatedAt = DateTime.UtcNow
            };
            var result = await _orderRepository.CreateOrderAsync(order);
            await _unitOfWork.SaveChangesAsync(); 

            var sendTo = _userProvider.GetCurrentUserEmail();
            var message = $"Hi {_userProvider.GetFullName()} your order with id: {order.OrderID} has been created";
            var subject = "SynthShop Order Creation";
            _emailProducer.SendMessage(new SendEmailMessage(subject, message, sendTo));
            await _basketRepository.DeleteBasketAsync(basketId);
            return result;
        }
        catch (DbUpdateConcurrencyException e)
        {
            _logger.Warning(e,
                "Failed to create order due to concurrency conflicts customerId: {customer}, basketId: {basket}",
                customerId, basketId);
            return new Result<Order>(e);
        }
    }

    public async Task<Result<Order>> CancelOrder(Guid orderId, Guid customerId)
    {
        var order = await _orderRepository.GetOrderAsync(orderId);

        if (order == null) return new Result<Order>(new InvalidOperationException("Order not found."));

        if (order.UserId != customerId)
        {
            _logger.Warning("User with id: {customerId} tried to modify other user's order userId: {orderUserId}",
                customerId, order.UserId);
            return new Result<Order>(new InvalidOperationException("Failed to cancel order"));
        }

        if (order.Status == OrderStatus.Completed)
            return new Result<Order>(new InvalidOperationException("Completed orders cannot be cancelled."));

        if (order.Status == OrderStatus.Pending)
            foreach (var item in order.OrderItems)
            {
                var product = item.Product;
                product.StockQuantity += item.Quantity;
                await _productRepository.UpdateAsync(product);
            }

        order.IsDeleted = true;
        order.Status = OrderStatus.Cancelled;

        var sendTo = _userProvider.GetCurrentUserEmail();
        var message = $"Hi {_userProvider.GetFullName()} your order with id: {order.OrderID} has been cancelled";
        var subject = "SynthShop Order Cancellation";
        _emailProducer.SendMessage(new SendEmailMessage(subject, message, sendTo));

        await _orderRepository.UpdateOrderAsync(orderId, order);
        await _unitOfWork.SaveChangesAsync();
        return order;
    }

    public async Task<Result<Order>> CompleteOrder(Guid orderId, Guid customerId)
    {
        var order = await _orderRepository.GetOrderAsync(orderId);

        if (order == null) return new Result<Order>(new InvalidOperationException("Order not found."));

        if (order.UserId != customerId)
            return new Result<Order>(new InvalidOperationException("User can't modify other user's order"));

        if (order.Status == OrderStatus.Completed)
            return new Result<Order>(new InvalidOperationException("Order is already completed."));


        if (order.Status == OrderStatus.Cancelled)
            return new Result<Order>(new InvalidOperationException("Cannot complete a cancelled order."));

        order.Status = OrderStatus.Completed;

        await _orderRepository.UpdateOrderAsync(orderId, order);
        await _unitOfWork.SaveChangesAsync();

        var sendTo = _userProvider.GetCurrentUserEmail();
        var message = $"Hi {_userProvider.GetFullName()} your order with id: {order.OrderID} has been completed";
        var subject = "SynthShop Order Completed";
        _emailProducer.SendMessage(new SendEmailMessage(subject, message, sendTo));


        return order;
    }

    public async Task<Result<List<Order>>> GetOrdersByCustomerId(Guid customerId)
    {
        try
        {
            var orders = await _orderRepository.GetOrdersByCustomerId(customerId);
            return orders;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving orders for customer ID: {CustomerId}", customerId);
            return new Result<List<Order>>(ex);
        }
    }

    public async Task<Result<Order>> GetOrderByIdAsync(Guid orderId)
    {
        var order = await _orderRepository.GetOrderAsync(orderId);
        return order != null
            ? new Result<Order>(order)
            : new Result<Order>(new InvalidOperationException("Order not found."));
    }
}