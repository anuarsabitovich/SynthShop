
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Interfaces
{
    public interface IOrderRepository 
    {
        Task<Order> CreateOrder(Guid basketId, Guid customerId);

        Task CancelOrder(Guid orderID);

        Task CompleteOrder(Guid orderId);
    }
}
