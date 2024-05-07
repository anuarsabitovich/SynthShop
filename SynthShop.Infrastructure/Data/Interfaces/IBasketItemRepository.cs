using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Interfaces
{
    public interface IBasketItemRepository
    {
        Task<Guid> CreateBasketItemAsync(BasketItem basketItem);
        Task<List<BasketItem?>> GetBasketItemsAsync();
        Task<BasketItem?> UpdateBasketItemAsync(Guid basketItemId, BasketItem updateBasketItem);
        Task DeleteBasketItem(Guid basketItemId);
    }
}
