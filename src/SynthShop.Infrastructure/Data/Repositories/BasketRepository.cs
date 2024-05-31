using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Infrastructure.Data.Repositories
{
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
            return await _dbContext.Baskets.Include(b=> b.Items).ThenInclude(bi => bi.Product).FirstOrDefaultAsync(b => b.BasketId == basketId);
        }

        public async Task DeleteBasketAsync(Guid basketId)
        {
            
            var basket = await _dbContext.Baskets
                .Include(b => b.Items) 
                .FirstOrDefaultAsync(b => b.BasketId == basketId);
            
            _dbContext.BasketItems.RemoveRange(basket.Items);
            _dbContext.Baskets.Remove(basket);
        }

    }
}
