using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;
using SynthShop.Infrastructure.Data.Repositories;

namespace SynthShop.Core.Services.Impl
{
    internal class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger _logger;
        private readonly IProductRepository _productRepository;
        private readonly IBasketItemRepository _basketItemRepository;
        
        public BasketService(IBasketRepository basketRepository, ILogger logger, IProductRepository productRepository, IBasketItemRepository basketItemRepository)
        {
            _basketRepository = basketRepository;
            _logger = logger;
            _productRepository = productRepository;
            _basketItemRepository = basketItemRepository;
        }
        public async Task<Guid> CreateBasketAsync()
        {
            var basketId = await _basketRepository.CreateBasketAsync();
            return basketId;
        }

        public async Task<Basket?> GetBasketByIdAsync(Guid basketId)
        {
            return await _basketRepository.GetBasketByIdAsync(basketId);
        }
        public async Task AddItemToBasketAsync(Guid basketId, Guid productId, int quantity)
        {

            var basket = await _basketRepository.GetBasketByIdAsync(basketId);
              
            if (basket == null)
            {
                _logger.Warning("Basket not found {basketId}", basketId);
                return;
            }

            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                _logger.Warning("Product not found {productId}", productId);
                return;
            }

            var existingItem = basket.Items
                .FirstOrDefault(item => item.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _basketItemRepository.UpdateBasketItemAsync(existingItem.BasketItemId, existingItem);
            }
            else
            {
                var newItem = new BasketItem
                {
                    BasketId = basketId,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _basketItemRepository.CreateBasketItemAsync(newItem);
            }
        }
     

        public async Task DeleteItemFromBasketAsync(Guid basketId, Guid basketItemId)
        {
            var basket = await _basketRepository.GetBasketByIdAsync(basketId);
            
            if (basket == null)
            {
                _logger.Warning("Basket with ID {BasketId} not found", basketId);
                return;
            }

            var itemToRemove = basket.Items.FirstOrDefault(i => i.BasketItemId == basketItemId);
            if (itemToRemove == null)
            {
                _logger.Warning("Item with ID {BasketItemId} not found in basket {BasketId}", basketItemId, basketId);
                return;
            }

            await _basketItemRepository.DeleteBasketItem(basketItemId);
            _logger.Information("Removed item {BasketItemId} from basket {BasketId}", basketItemId, basketId);

        }

        public async Task DeleteBasketAsync(Guid basketId)
        {
            var basket = await _basketRepository.GetBasketByIdAsync(basketId);
            if (basket == null)
            {
                _logger.Warning("Basket {BasketId} not found",  basketId);

                return;
            }
            await _basketRepository.DeleteBasketAsync(basketId);
        }

        public async Task UpdateItemInBasket(Guid basketId, Guid basketItemId, int quantity)
        {

            var basket = await _basketRepository.GetBasketByIdAsync(basketId);
            if (basket == null)
            {
                _logger.Warning("Basket with ID {BasketId} not found", basketId);
                return;
            }

            var basketItem = basket.Items.FirstOrDefault(item => item.BasketItemId == basketItemId);
            if (basketItem == null)
            {
                _logger.Warning("Item with ID {BasketItemId} not found in basket {BasketId}", basketItemId, basketId);
                return;
            }
            basketItem.Quantity = quantity;

            await _basketItemRepository.UpdateBasketItemAsync(basketItemId, basketItem);
        }

        public Task<bool> SendBasketForCheckoutAsync(Guid basketId) // TODO change name Complete order
        {
            throw new NotImplementedException();
        }
    }
}
