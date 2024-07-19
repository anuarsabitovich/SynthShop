using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Infrastructure.Data.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly MainDbContext _dbContext;

    public BasketRepository(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task CreateBasketAsync(Basket basket)
    {
        await _dbContext.Baskets.AddAsync(basket);
    }

    public async Task<Basket?> GetBasketByIdAsync(Guid basketId)
    {
        return await _dbContext.Baskets.Include(b => b.Items).ThenInclude(bi => bi.Product)
            .FirstOrDefaultAsync(b => b.BasketId == basketId);
    }

    public async Task DeleteBasketAsync(Guid basketId)
    {
        var basket = await _dbContext.Baskets
            .Include(b => b.Items)
            .FirstOrDefaultAsync(b => b.BasketId == basketId);

        _dbContext.BasketItems.RemoveRange(basket.Items);
        _dbContext.Baskets.Remove(basket);
    }

    public async Task<Basket?> UpdateBasketAsync(Guid basketId, Guid? customerId)
    {
        var basket = await _dbContext.Baskets.Include(b => b.Items).ThenInclude(bi => bi.Product)
            .FirstOrDefaultAsync(b => b.BasketId == basketId);

        if (basket == null) return null;

        basket.CustomerId = customerId;

        _dbContext.Baskets.Update(basket);
        await _dbContext.SaveChangesAsync();

        return basket;
    }

    public async Task<Basket?> GetLastBasketByCustomerIdAsync(Guid customerId) 
    {
        return await _dbContext.Baskets
            .Include(b => b.Items).ThenInclude(bi => bi.Product)
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.BasketId) 
            .FirstOrDefaultAsync();
    }
}