using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;
using SynthShop.Infrastructure.Data.Repositories;

namespace SynthShop.Core.Services.Impl
{
    internal class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        public BasketService(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }
        public async Task CreateBasketAsync()
        {
            await _basketRepository.CreateBasketAsync();
        }

        public async Task<Basket?> GetBasketByIdAsync(Guid basketId)
        {
            return await _basketRepository.GetBasketByIdAsync(basketId);
        }

        public async Task AddItemToBasketAsync(Guid basketId, Guid productId, int quantity)
        {
            await _basketRepository.AddItemToBasketAsync(basketId, productId, quantity);
        }

        public async Task DeleteItemFromBasketAsync(Guid basketId, Guid basketItemId)
        {
            await _basketRepository.DeleteItemFromBasketAsync(basketId, basketItemId);
        }

        public async Task DeleteBasketAsync(Guid basketId)
        {
            await _basketRepository.DeleteBasketAsync(basketId);
        }

        public async Task UpdateItemInBasket(Guid basketId, Guid basketItemId, int quantity)
        {
            await _basketRepository.UpdateItemInBasket(basketId, basketItemId, quantity);
        }

        public Task<bool> SendBasketForCheckoutAsync(Guid basketId) // TODO change name Complete order
        {
            throw new NotImplementedException();
        }
    }
}
