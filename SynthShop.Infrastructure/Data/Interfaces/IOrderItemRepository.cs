
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<OrderItem> CreateAsync(OrderItem orderItem);
        Task<List<OrderItem>> GetAllAsync();
        Task<OrderItem?> GetByIdAsync(Guid id);
        Task<OrderItem?> UpdateAsync(Guid id, OrderItem orderItem);
        Task<OrderItem?> DeleteAsync(Guid id);  
    }
}
