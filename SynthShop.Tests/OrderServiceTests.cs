using NSubstitute;
using SynthShop.Core.Services.Impl;
using SynthShop.Infrastructure.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Enums;

namespace SynthShop.Tests
{
    public class OrderServiceTests
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger _logger;
        private readonly OrderService _sut;

        public OrderServiceTests()
        {
            _orderRepository = Substitute.For<IOrderRepository>();
            _basketRepository = Substitute.For<IBasketRepository>();
            _logger = Substitute.For<ILogger>();
            _sut = new OrderService(_orderRepository, _basketRepository, _logger);
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
        public async Task CancelOrder_ShouldThrowException_WhenOrderNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult<Order>(null));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CancelOrder(orderId));
            Assert.Equal("Order not found.", exception.Message);
        }

        [Fact]
        public async Task CancelOrder_ShouldThrowException_WhenOrderIsCompleted()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { OrderID = orderId, Status = OrderStatus.Completed };
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CancelOrder(orderId));
            Assert.Equal("Completed orders cannot be cancelled.", exception.Message);
        }

        [Fact]
        public async Task CancelOrder_ShouldCancelPendingOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                OrderID = orderId,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Product = new Product { StockQuantity = 10 },
                        Quantity = 5
                    }
                }
            };
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

            // Act
            await _sut.CancelOrder(orderId);

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.Equal(15, order.OrderItems.First().Product.StockQuantity); // Stock should be restored
            await _orderRepository.Received(1).DeleteOrderAsync(orderId);
        }

        [Fact]
        public async Task CompleteOrder_ShouldThrowException_WhenOrderNotFound()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult<Order>(null));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CompleteOrder(orderId));
            Assert.Equal("Order not found.", exception.Message);
        }

        [Fact]
        public async Task CompleteOrder_ShouldThrowException_WhenOrderIsAlreadyCompleted()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { OrderID = orderId, Status = OrderStatus.Completed };
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CompleteOrder(orderId));
            Assert.Equal("Order is already completed.", exception.Message);
        }

        [Fact]
        public async Task CompleteOrder_ShouldThrowException_WhenOrderIsCancelled()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { OrderID = orderId, Status = OrderStatus.Cancelled };
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CompleteOrder(orderId));
            Assert.Equal("Cannot complete a cancelled order.", exception.Message);
        }

        [Fact]
        public async Task CompleteOrder_ShouldCompletePendingOrder()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order { OrderID = orderId, Status = OrderStatus.Pending };
            _orderRepository.GetOrderAsync(orderId).Returns(Task.FromResult(order));

            // Act
            await _sut.CompleteOrder(orderId);

            // Assert
            Assert.Equal(OrderStatus.Completed, order.Status);
            await _orderRepository.Received(1).UpdateOrderAsync(orderId, order);
        }
    }
}
