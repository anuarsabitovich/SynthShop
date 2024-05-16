using SynthShop.Domain.Entities;

namespace SynthShop.Core.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrder(Guid basketId, Guid customerId);
        
        Task CancelOrder(Guid orderId, Guid customerId);

        Task CompleteOrder(Guid basketId, Guid customerId);
    }
}
