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
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemService(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        public async Task CreateAsync(OrderItem orderItem)
        {
            await _orderItemRepository.CreateAsync(orderItem);
        }

        public async Task<List<OrderItem>> GetAllAsync()
        {
            return await _orderItemRepository.GetAllAsync();
        }

        public async Task<OrderItem?> GetByIdAsync(Guid id)
        {
            return await _orderItemRepository.GetByIdAsync(id);
        }

        public async Task<OrderItem?> UpdateAsync(Guid id, OrderItem orderItem)
        {
            return await _orderItemRepository.UpdateAsync(id, orderItem);
        }

        public async Task<OrderItem?> DeleteAsync(Guid id)
        {
            return await _orderItemRepository.DeleteAsync(id);
        }
    }
}
