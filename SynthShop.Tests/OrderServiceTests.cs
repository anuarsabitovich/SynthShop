﻿using NSubstitute;
using Serilog;
using SynthShop.Core.Services.Impl;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Enums;
using SynthShop.Infrastructure.Data.Interfaces;
using Xunit;

namespace SynthShop.Tests
{
    public class OrderServiceTests
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger _logger;
        private readonly OrderService _sut;
        private readonly IUnitOfWork _unitOfWork;

        public OrderServiceTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _basketRepository = Substitute.For<IBasketRepository>();
            _logger = Substitute.For<ILogger>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _sut = new OrderService(_orderRepository, _basketRepository, _logger, _unitOfWork);
        }

        [Fact]
        public async Task CreateOrder_ShouldThrowException_WhenBasketNotFound()
        {
            // Arrange
            var basketId = Guid.NewGuid();
            _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult<Basket>(null));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateOrder(basketId, Guid.NewGuid()));
            Assert.Equal("Basket not found.", exception.Message);
        }

        [Fact]
        public async Task CreateOrder_ShouldThrowException_WhenBasketIsEmpty()
        {
            // Arrange
            var basketId = Guid.NewGuid();
            var basket = new Basket
            {
                BasketId = basketId,
                Items = new List<BasketItem>()
            };
            _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateOrder(basketId, Guid.NewGuid()));
            Assert.Equal("Basket is empty", exception.Message);
        }

        [Fact]
        public async Task CreateOrder_ShouldThrowException_WhenProductAvailabilityIsInsufficient()
        {
            // Arrange
            var basketId = Guid.NewGuid();
            var basket = new Basket
            {
                BasketId = basketId,
                Items = new List<BasketItem>
                {
                    new BasketItem
                    {
                        Product = new Product { Name = "Product1", StockQuantity = 5 },
                        Quantity = 10
                    }
                }
            };
            _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateOrder(basketId, Guid.NewGuid()));
            Assert.Contains("Not enough stock for Product1", exception.Message);
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
                    new BasketItem
                    {
                        Product = new Product { Name = "Product1", StockQuantity = 10, Price = 100 },
                        Quantity = 5
                    }
                }
            };
            _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));
            _orderRepository.CreateOrderAsync(Arg.Any<Order>()).Returns(ci => ci.Arg<Order>());
            _unitOfWork.SaveChangesAsync().Returns(Task.CompletedTask);

            // Act
            var result = await _sut.CreateOrder(basketId, customerId);

            // Assert
            Assert.Equal(OrderStatus.Pending, result.Status);
            Assert.Equal(customerId, result.UserId);
            Assert.Single(result.OrderItems);
            Assert.Equal(5, result.OrderItems.First().Quantity);
            await _orderRepository.Received(1).CreateOrderAsync(Arg.Any<Order>());
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CancelOrder_ShouldThrowException_WhenOrderNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult<Order>(null));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CancelOrder(orderId, customerId));
            Assert.Equal("Order not found.", exception.Message);
        }

        [Fact]
        public async Task CancelOrder_ShouldThrowException_WhenUserCannotModifyOtherUsersOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var otherCustomerId = Guid.NewGuid();
            var order = new Order { OrderID = orderId, UserId = otherCustomerId, Status = OrderStatus.Pending };
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CancelOrder(orderId, customerId));
            Assert.Equal("User can't modify other user's order", exception.Message);
        }

        [Fact]
        public async Task CancelOrder_ShouldThrowException_WhenOrderIsCompleted()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var order = new Order { OrderID = orderId, UserId = customerId, Status = OrderStatus.Completed };
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CancelOrder(orderId, customerId));
            Assert.Equal("Completed orders cannot be cancelled.", exception.Message);
        }

        [Fact]
        public async Task CancelOrder_ShouldCancelPendingOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var product = new Product { StockQuantity = 10 };
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
                        Quantity = 5
                    }
                }
            };

            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));
            _unitOfWork.SaveChangesAsync().Returns(Task.CompletedTask);
            _orderRepository.DeleteOrderAsync(orderId).Returns(Task.CompletedTask);

            // Act
            await _sut.CancelOrder(orderId, customerId);

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.Equal(15, order.OrderItems.First().Product.StockQuantity); // Stock should be restored
            await _orderRepository.Received(1).DeleteOrderAsync(orderId);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task CompleteOrder_ShouldThrowException_WhenOrderNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult<Order>(null));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CompleteOrder(orderId, customerId));
            Assert.Equal("Order not found.", exception.Message);
        }

        [Fact]
        public async Task CompleteOrder_ShouldThrowException_WhenUserCannotModifyOtherUsersOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var otherCustomerId = Guid.NewGuid();
            var order = new Order { OrderID = orderId, UserId = otherCustomerId, Status = OrderStatus.Pending };
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CompleteOrder(orderId, customerId));
            Assert.Equal("User can't modify other user's order", exception.Message);
        }

        [Fact]
        public async Task CompleteOrder_ShouldThrowException_WhenOrderIsAlreadyCompleted()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var order = new Order { OrderID = orderId, UserId = customerId, Status = OrderStatus.Completed };
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CompleteOrder(orderId, customerId));
            Assert.Equal("Order is already completed.", exception.Message);
        }

        [Fact]
        public async Task CompleteOrder_ShouldThrowException_WhenOrderIsCancelled()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var order = new Order { OrderID = orderId, UserId = customerId, Status = OrderStatus.Cancelled };
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CompleteOrder(orderId, customerId));
            Assert.Equal("Cannot complete a cancelled order.", exception.Message);
        }
    }
}