using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Interfaces
{
    public interface IBasketItemRepository
    {
        Task<BasketItem> CreateBasketItemAsync(BasketItem basketItem);
        Task<List<BasketItem?>> GetBasketItemsAsync();
        Task<BasketItem?> UpdateBasketItemAsync(Guid basketItemId, BasketItem updateBasketItem);
        Task DeleteBasketItem(Guid basketItemId);
        Task<BasketItem?> GetBasketItemByIdAsync(Guid basketItemId);
    }
}
