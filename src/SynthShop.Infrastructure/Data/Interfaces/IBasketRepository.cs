using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Interfaces;

public interface IBasketRepository
{
    Task CreateBasketAsync(Basket basket);
    Task<Basket?> GetBasketByIdAsync(Guid basketId);
    Task DeleteBasketAsync(Guid basketId);
    Task<Basket?> UpdateBasketAsync(Guid basketId, Guid? customerId);
    Task<Basket?> GetLastBasketByCustomerIdAsync(Guid customerId);
}