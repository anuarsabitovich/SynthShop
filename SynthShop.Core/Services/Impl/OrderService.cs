using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Domain.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthShop.Core.Services.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task CreateAsync(Order order)
        {
            await _orderRepository.CreateAsync(order);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _orderRepository.GetAllAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _orderRepository.GetByIdAsync(id);
        }

        public async Task<Order?> UpdateAsync(Guid id, Order order)
        {
            return await _orderRepository.UpdateAsync(id, order);
        }

        public async Task<Order?> DeleteAsync(Guid id)
        {
            return await _orderRepository.DeleteAsync(id);
        }


    }
}
