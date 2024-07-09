using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Infrastructure.Data.Repositories;

public class BasketItemRepository : IBasketItemRepository
{
    private readonly MainDbContext _dbContext;

    public BasketItemRepository(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BasketItem> CreateBasketItemAsync(BasketItem basketItem)
    {
        await _dbContext.BasketItems.AddAsync(basketItem);
        return basketItem;
    }

    public async Task<List<BasketItem?>> GetBasketItemsAsync()
    {
        return await _dbContext.BasketItems.AsNoTracking().ToListAsync();
    }

    public async Task<BasketItem?> GetBasketItemByIdAsync(Guid basketItemId)
    {
        return await _dbContext.BasketItems.FirstOrDefaultAsync(x => x.BasketItemId == basketItemId);
    }

    public async Task<BasketItem?> UpdateBasketItemAsync(Guid basketItemId, BasketItem updateBasketItem)
    {
        var existingBasketItem = await _dbContext.BasketItems.FirstAsync(bi => bi.BasketItemId == basketItemId);

        existingBasketItem.Quantity = updateBasketItem.Quantity;

        _dbContext.BasketItems.Update(existingBasketItem);

        return existingBasketItem;
    }

    public async Task DeleteBasketItem(Guid basketItemId)
    {
        var basketItem = await _dbContext.BasketItems.FirstAsync(bi => bi.BasketItemId == basketItemId);

        _dbContext.BasketItems.Remove(basketItem);
    }
}