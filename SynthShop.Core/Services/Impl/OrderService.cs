using Microsoft.EntityFrameworkCore;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Enums;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Core.Services.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger _logger;


        public OrderService(IOrderRepository orderRepository, IBasketRepository basketRepository, ILogger logger)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _logger = logger;
        }

        public async Task<Order> CreateOrder(Guid basketId, Guid customerId) 
        {
            try
            {
                var basket = await _basketRepository.GetBasketByIdAsync(basketId);


                if (basket == null)
                    throw new InvalidOperationException("Basket not found.");

                if (!basket.CustomerId.HasValue)
                {
                    basket.CustomerId = customerId;
                }

                var availabilityIssues = new List<string>();
                foreach (var item in basket.Items)
                {
                    if (item.Product.StockQuantity < item.Quantity)
                    {
                        availabilityIssues.Add($"Not enough stock for {item.Product.Name}. Requested: {item.Quantity}, Available: {item.Product.StockQuantity}");
                    }
                    else
                    {
                        item.Product.StockQuantity -= item.Quantity;
                    }
                }

                if (availabilityIssues.Any())
                {
                    _logger.Warning("Failed to create order due to concurrency conflicts for basket ID {BasketId}", basketId);

                    throw new InvalidOperationException("There are issues with product availability: " + string.Join(", ", availabilityIssues));
                }

                var order = new Order
                {
                    OrderID = Guid.NewGuid(),
                    OrderDate = DateTime.UtcNow,
                    UserId = basket.CustomerId.Value,
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

                return await _orderRepository.CreateOrderAsync(order);

            }
            catch (DbUpdateConcurrencyException)
            {
                // warning
                
                throw new InvalidOperationException("Failed to create order due to concurrency conflicts.");
            }
        }

        public async Task CancelOrder(Guid orderId)
        {
            var order = await _orderRepository.GetOrderAsync(orderId);


            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            if (order.Status == OrderStatus.Completed)
            {
                throw new InvalidOperationException("Completed orders cannot be cancelled.");
            }


            if (order.Status == OrderStatus.Pending)
            {
                foreach (var item in order.OrderItems)
                {
                    var product = item.Product;
                    product.StockQuantity += item.Quantity;
                }
            }


            order.Status = OrderStatus.Cancelled;

            await _orderRepository.DeleteOrderAsync(orderId);
        }

        public async Task CompleteOrder(Guid orderId)
        {
            var order = await _orderRepository.GetOrderAsync(orderId);

            if (order == null)
            {
                throw new InvalidOperationException("Order not found.");
            }

            if (order.Status == OrderStatus.Completed)
            {
                throw new InvalidOperationException("Order is already completed.");
            }

            if (order.Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot complete a cancelled order.");
            }

            order.Status = OrderStatus.Completed;

            await _orderRepository.UpdateOrderAsync(orderId, order);
        }
    }
}
