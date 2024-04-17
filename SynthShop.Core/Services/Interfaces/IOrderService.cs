using SynthShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthShop.Core.Services.Interfaces
{
    public interface IOrderService
    {
        Task CreateAsync(Order order);
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(Guid id);
        Task<Order?> UpdateAsync(Guid id, Order order);
        Task<Order?> DeleteAsync(Guid id);
    }
}
