using SynthShop.Data.Entities;

namespace SynthShop.Repositories
{
    public interface IOrderItemRepository
    {
        Task<OrderItem> CreateAsync(OrderItem orderItem);
        Task<List<OrderItem>> GetAllAsync();
        Task<OrderItem?> GetByIdASync(Guid id);
        Task<OrderItem?> UpdateAsync(Guid id, OrderItem orderItem);
        Task<OrderItem?> DeleteAsync(Guid id);  
    }
}
