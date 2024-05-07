using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Infrastructure.Data.Repositories
{
    public class BasketItemRepository : IBasketItemRepository
    {
        private readonly MainDbContext _dbContext;

        public BasketItemRepository(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid> CreateBasketItemAsync(BasketItem basketItem)
        {
            await _dbContext.BasketItems.AddAsync(basketItem);
            await _dbContext.SaveChangesAsync();
            return basketItem.BasketItemId;
        }

        public async Task<List<BasketItem?>> GetBasketItemsAsync()
        {
            return await _dbContext.BasketItems.AsNoTracking().ToListAsync();
        }

        public async Task<BasketItem?> GetBasketItemByIdAsync(Guid basketItemId, BasketItem updateBasketItem)
        {
            return await _dbContext.BasketItems.FirstOrDefaultAsync(x => x.BasketItemId == basketItemId);
        }

        public async Task<BasketItem?> UpdateBasketItemAsync(Guid basketItemId, BasketItem updateBasketItem)
        {
            var existingBasketItem = await _dbContext.BasketItems.FirstOrDefaultAsync(bi => bi.BasketItemId == basketItemId);

            existingBasketItem.Quantity = updateBasketItem.Quantity;

            _dbContext.BasketItems.Update(existingBasketItem);
            await _dbContext.SaveChangesAsync();

            return existingBasketItem;
        }

        public async Task DeleteBasketItem(Guid basketItemId)
        {
            var basketItem = await _dbContext.BasketItems.FirstOrDefaultAsync(bi => bi.BasketItemId == basketItemId);

            _dbContext.BasketItems.Remove(basketItem);

            await _dbContext.SaveChangesAsync();
            
        }
    }
}
