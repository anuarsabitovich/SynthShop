using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Infrastructure.Data.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger _logger;

        public BasketRepository(MainDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger.ForContext<BasketRepository>();
        }


        public async Task<Guid> CreateBasketAsync()
        {
            var basket = new Basket();
            await _dbContext.Baskets.AddAsync(basket);
            await _dbContext.SaveChangesAsync();
            return basket.BasketId;
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
            await _dbContext.SaveChangesAsync();
        }

    }
}
