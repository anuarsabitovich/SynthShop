using SynthShop.Data.Entities;

namespace SynthShop.Repositories
{
    public interface IOrderRepository 
    {
        Task<Order> CreateAsync(Order order);
        Task<List<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(Guid id);
        Task<Order?> UpdateAsync(Guid id, Order order);
        Task<Order?> DeleteAsync(Guid id);
    }
}
