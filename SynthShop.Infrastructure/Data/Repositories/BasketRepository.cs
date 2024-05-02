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
    public class BasketRepository : IBasketRepository
    {
        private readonly MainDbContext _dbContext;

        public BasketRepository(MainDbContext dbContext)
        {
            _dbContext = dbContext;
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
            var basket = await _dbContext.Baskets.FirstOrDefaultAsync(b => b.BasketId == basketId);
            if (basket == null)
            {
                throw new InvalidOperationException("Basket not found");
            }
            basket.Items = await _dbContext.BasketItems.Include(x => x.Product).Where(item => item.BasketId == basketId).ToListAsync();
            return basket;
        }
        public async Task AddItemToBasketAsync(Guid basketId, Guid productId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }

            var basket = await _dbContext.Baskets
                .Include(b => b.Items)
                .SingleOrDefaultAsync(b => b.BasketId == basketId);
            if (basket == null)
            {
                throw new InvalidOperationException("Basket not found");
            }

            var existingItem = basket.Items
                .FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new BasketItem
                {
                    BasketId = basketId,
                    ProductId = productId,
                    Quantity = quantity
                };
                basket.Items.Add(newItem);
            }

            await _dbContext.SaveChangesAsync();
        }


        public async Task DeleteItemFromBasketAsync(Guid basketId, Guid basketItemId)
        {
            var basket = await _dbContext.Baskets.Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.BasketId == basketId);
            if (basket == null)
            {
                throw new InvalidOperationException("Basket not found");
            }

            if (basket.Items == null)
            {
                throw new InvalidOperationException("Basket is empty");
            }

            var itemRemove = basket.Items.FirstOrDefault(i => i.BasketItemId == basketItemId);
            {
                if (itemRemove == null)
                {
                    throw new InvalidOperationException("Item is not in basket");
                }
                basket.Items.Remove(itemRemove);

                await _dbContext.SaveChangesAsync();
            }

        }

        public async Task UpdateItemInBasket(Guid basketId, Guid basketItemId, int quantity)
        {
            var basket = await _dbContext.Baskets
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.BasketId == basketId);

            if (basket == null)
            {
                throw new InvalidOperationException("Basket not found");
            }

            var basketItem = basket.Items.FirstOrDefault(item => item.BasketItemId == basketItemId);

            if (basketItem == null)
            {
                throw new InvalidOperationException("Basket item not found");
            }

            basketItem.Quantity = quantity;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteBasketAsync(Guid basketId)
        {
            var basket = await _dbContext.Baskets
                .Include(b => b.Items) 
                .FirstOrDefaultAsync(b => b.BasketId == basketId);
            if (basket == null)
            {
                throw new InvalidOperationException("Basket doesn't exist");
            }

    
            _dbContext.BasketItems.RemoveRange(basket.Items);

          
            _dbContext.Baskets.Remove(basket);
            await _dbContext.SaveChangesAsync();
        }


    }
}
