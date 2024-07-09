using LanguageExt.Common;
using SynthShop.Domain.Entities;

namespace SynthShop.Core.Services.Interfaces;

public interface IOrderService
{
    Task<Result<Order>> CreateOrder(Guid basketId, Guid customerId);

    Task<Result<Order>> CancelOrder(Guid orderId, Guid customerId);

    Task<Result<Order>> CompleteOrder(Guid basketId, Guid customerId);
    Task<Result<List<Order>>> GetOrdersByCustomerId(Guid customerId);
    Task<Result<Order>> GetOrderByIdAsync(Guid orderId);
}