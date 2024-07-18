using NSubstitute;
using Serilog;
using SynthShop.Core.Services.Impl;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Enums;
using SynthShop.Domain.Exceptions;
using SynthShop.Infrastructure.Data.Interfaces;
using SynthShop.Tests.Extensions;
using System;
using LanguageExt;
using NSubstitute.ExceptionExtensions;

namespace SynthShop.Tests;

public class OrderServiceTests
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger _logger;
    private readonly OrderService _sut;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IEmailProducer _emailProducer;
    private readonly IUserProvider _userProvider;


    public OrderServiceTests()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _basketRepository = Substitute.For<IBasketRepository>();
        _logger = Substitute.For<ILogger>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _productRepository = Substitute.For<IProductRepository>();
        _userProvider = Substitute.For<IUserProvider>();
        _emailProducer = Substitute.For<IEmailProducer>();
        _sut = new OrderService(_orderRepository, _basketRepository, _logger, _unitOfWork, _productRepository, _emailProducer, _userProvider);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnError_WhenBasketNotFound()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult<Basket>(null));

        // Act
        var result = await _sut.CreateOrder(basketId, Guid.NewGuid());
        
        // Assert
        Assert.False(result.IsSuccess);
        result.Match<Unit>(
            value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return Unit.Default;
            },
            ex =>
            {
                var exception = Assert.IsType<OrderFailedException>(ex);
                Assert.Equal("Basket not found.", exception.Message);
                return Unit.Default;
            });
    }


    [Fact]
    public async Task CreateOrder_ShouldReturnError_WhenBasketIsEmpty()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basket = new Basket
        {
            BasketId = basketId,
            Items = new List<BasketItem>()
        };
        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));

        // Act
        var result = await _sut.CreateOrder(basketId, Guid.NewGuid());

        // Assert
        Assert.False(result.IsSuccess);
        result.Match<Unit>(
            value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return Unit.Default;
            },
            ex =>
            {
                var exception = Assert.IsType<OrderFailedException>(ex);
                Assert.Equal("Basket is empty.", exception.Message);
                return Unit.Default;
            });
      
    }


    [Fact]
    public async Task CreateOrder_ShouldReturnError_WhenProductAvailabilityIsInsufficient()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basket = new Basket
        {
            BasketId = basketId,
            Items = new List<BasketItem>
            {
                new()
                {
                    Product = new Product { Name = "Product1", StockQuantity = 5 },
                    Quantity = 10
                }
            }
        };
        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));

        // Act
        var result = await _sut.CreateOrder(basketId, Guid.NewGuid());
        

        // Assert
        Assert.False(result.IsSuccess);
        result.Match<Unit>(
            value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return Unit.Default;
            },
            ex =>
            {
                var exception = Assert.IsType<OrderFailedException>(ex);
                Assert.Contains("Not enough stock for Product1", exception.Message);
                return Unit.Default;
            });
    }


    [Fact]
    public async Task CreateOrder_ShouldCreateOrderSuccessfully()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var basket = new Basket
        {
            BasketId = basketId,
            CustomerId = customerId,
            Items = new List<BasketItem>
            {
                new()
                {
                    Product = new Product { Name = "Product1", StockQuantity = 10, Price = 100 },
                    Quantity = 5
                }
            }
        };
        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));
        _orderRepository.CreateOrderAsync(Arg.Any<Order>()).Returns(ci => ci.Arg<Order>());
        _unitOfWork.SaveChangesAsync().Returns(Task.CompletedTask);
        _userProvider.GetCurrentUserEmail().Returns("test@example.com");
        _userProvider.GetFullName().Returns("Test User");
        
        // Act
        var result = await _sut.CreateOrder(basketId, customerId);
        var orderResult = ResultObjectExtension.UnwrapResult(result);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Pending, orderResult.Status);
        Assert.Equal(customerId, orderResult.UserId);
        Assert.Single(orderResult.OrderItems);
        Assert.Equal(5, orderResult.OrderItems.First().Quantity);
        await _orderRepository.Received(1).CreateOrderAsync(Arg.Any<Order>());
        await _unitOfWork.Received(1).SaveChangesAsync();
    }


    [Fact]
    public async Task CancelOrder_ShouldReturnError_WhenOrderNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult<Order>(null));

        // Act
        var result = await _sut.CancelOrder(orderId, customerId);

        // Assert
        Assert.False(result.IsSuccess);
        result.Match<Unit>(
            value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return Unit.Default;
            },
            ex =>
            {
                var exception = Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("Order not found.", exception.Message);
                return Unit.Default;
            });
    }


    [Fact]
    public async Task CancelOrder_ShouldReturnError_WhenUserCannotModifyOtherUsersOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var otherCustomerId = Guid.NewGuid();
        var order = new Order { OrderID = orderId, UserId = otherCustomerId, Status = OrderStatus.Pending };
        _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

        // Act
        var result = await _sut.CancelOrder(orderId, customerId);

        // Assert
        Assert.False(result.IsSuccess);
        result.Match<Unit>(
            value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return Unit.Default;
            },
            ex =>
            {
                var exception = Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("Failed to cancel order", exception.Message);
                return Unit.Default;
            });
    }

    [Fact]
    public async Task CancelOrder_ShouldReturnError_WhenOrderIsCompleted()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var order = new Order { OrderID = orderId, UserId = customerId, Status = OrderStatus.Completed };
        _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

        // Act
        var result = await _sut.CancelOrder(orderId, customerId);

        // Assert
        Assert.False(result.IsSuccess);
        result.Match<Unit>(
            value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return Unit.Default;
            },
            ex =>
            {
                var exception = Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("Completed orders cannot be cancelled.", exception.Message);
                return Unit.Default;
            });
    }

    [Fact]
    public async Task CancelOrder_ShouldCancelPendingOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product { ProductID = productId, StockQuantity = 10, Price = 100 };
        var order = new Order
        {
            OrderID = orderId,
            UserId = customerId,
            Status = OrderStatus.Pending,
            OrderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    Product = product,
                    Quantity = 5,
                    ProductID = productId
                }
            }
        };

        _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));
        _unitOfWork.SaveChangesAsync().Returns(Task.CompletedTask);
        _productRepository.UpdateAsync(Arg.Any<Product>()).Returns(ci => Task.FromResult(ci.Arg<Product>()));
        _orderRepository.UpdateOrderAsync(orderId, order).Returns(Task.FromResult(order));

        // Act
        var result = await _sut.CancelOrder(orderId, customerId);
        var orderResult = result.Match(
            value => value,
            ex => throw ex);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Cancelled, orderResult.Status);
        Assert.True(orderResult.IsDeleted);
        Assert.Equal(15, orderResult.OrderItems.First().Product.StockQuantity); // Stock should be restored
        await _productRepository.Received(1).UpdateAsync(Arg.Is<Product>(p => p.ProductID == productId && p.StockQuantity == 15));
        await _unitOfWork.Received(1).SaveChangesAsync();
    }




    [Fact]
    public async Task CompleteOrder_ShouldReturnError_WhenOrderNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult<Order>(null));

        // Act
        var result = await _sut.CompleteOrder(orderId, customerId);

        // Assert
        Assert.False(result.IsSuccess);
        result.Match<Unit>(
            value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return Unit.Default;
            },
            ex =>
            {
                var exception = Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("Order not found.", exception.Message);
                return Unit.Default;
            });
    }


    [Fact]
    public async Task CompleteOrder_ShouldReturnError_WhenUserCannotModifyOtherUsersOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var otherCustomerId = Guid.NewGuid();
        var order = new Order { OrderID = orderId, UserId = otherCustomerId, Status = OrderStatus.Pending };
        _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

        // Act
        var result = await _sut.CompleteOrder(orderId, customerId);

        // Assert
        Assert.False(result.IsSuccess);
        result.Match<Unit>(
            value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return Unit.Default;
            },
            ex =>
            {
                var exception = Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("User can't modify other user's order", exception.Message);
                return Unit.Default;
            });
    }


    [Fact]
    public async Task CompleteOrder_ShouldReturnError_WhenOrderIsAlreadyCompleted()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var order = new Order { OrderID = orderId, UserId = customerId, Status = OrderStatus.Completed };
        _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

        // Act
        var result = await _sut.CompleteOrder(orderId, customerId);

        // Assert
        Assert.False(result.IsSuccess);
        result.Match<Unit>(
            value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return Unit.Default;
            },
            ex =>
            {
                var exception = Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("Order is already completed.", exception.Message);
                return Unit.Default;
            });
    }


    [Fact]
    public async Task CompleteOrder_ShouldReturnError_WhenOrderIsCancelled()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var order = new Order { OrderID = orderId, UserId = customerId, Status = OrderStatus.Cancelled };
        _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

        // Act
        var result = await _sut.CompleteOrder(orderId, customerId);

        // Assert
        Assert.False(result.IsSuccess);
        result.Match<Unit>(
            value =>
            {
                Assert.Fail("Expected failure, but got success.");
                return Unit.Default;
            },
            ex =>
            {
                var exception = Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal("Cannot complete a cancelled order.", exception.Message);
                return Unit.Default;
            });
    }

}