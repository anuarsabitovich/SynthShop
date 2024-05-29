using SynthShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthShop.Core.Services.Interfaces
{
    public interface IOrderItemService
    {
        Task CreateAsync(OrderItem orderItem);
        Task<List<OrderItem>> GetAllAsync();
        Task<OrderItem?> GetByIdAsync(Guid id);
        Task<OrderItem?> UpdateAsync(Guid id, OrderItem orderItem);
        Task<OrderItem?> DeleteAsync(Guid id);
    }
}
