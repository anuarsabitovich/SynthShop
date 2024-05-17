using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Interfaces
{
    public interface IOrderRepository 
    {
        Task<Order?> GetOrderAsync(Guid orderId);
        
        Task<Order> CreateOrderAsync(Order order);

        Task DeleteOrderAsync(Guid orderId);

        Task<Order?> UpdateOrderAsync(Guid id, Order order);
    }
}
