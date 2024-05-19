using SynthShop.Domain.Entities;

namespace SynthShop.Core.Services.Interfaces
{
    public interface IBasketService
    {
        Task<Guid> CreateBasketAsync();
        Task<Basket?> GetBasketByIdAsync(Guid basketId);

        Task AddItemToBasketAsync(Guid basketId, Guid productId, int quantity);

        Task DeleteItemFromBasketAsync(Guid basketId, Guid basketItemId);

        Task DeleteBasketAsync(Guid basketId);
        Task UpdateItemInBasket(Guid basketId, Guid basketItemId, int quantity);
    }
}
