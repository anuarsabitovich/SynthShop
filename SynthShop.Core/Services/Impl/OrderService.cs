using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Core.Services.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBasketRepository _basketRepository;


        public OrderService(IOrderRepository orderRepository, IBasketRepository basketRepository)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
        }

        public async Task<Order> CreateOrder(Guid basketId, Guid customerId)
        {
            var order = await _orderRepository.CreateOrder(basketId, customerId);
            return order;
        }

        public async Task CancelOrder(Guid orderID)
        {
            await _orderRepository.CancelOrder(orderID);
        }

        public async Task CompleteOrder(Guid orderId)
        {
            await _orderRepository.CompleteOrder(orderId);
        }
    }
}
