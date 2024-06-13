using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Interfaces
{
    public interface IOrderRepository 
    {
        Task<Order?> GetOrderAsync(Guid orderId);
        
        Task<Order> CreateOrderAsync(Order order);

        Task DeleteOrderAsync(Guid orderId);
        Task<List<Order>> GetOrdersByCustomerId(Guid customerId);

        Task<Order?> UpdateOrderAsync(Guid id, Order order);
    }
}
