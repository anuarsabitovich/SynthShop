using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Core.Services.Impl
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger _logger;
        private readonly IProductRepository _productRepository;
        private readonly IBasketItemRepository _basketItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        
        public BasketService(IBasketRepository basketRepository, ILogger logger, IProductRepository productRepository, IBasketItemRepository basketItemRepository, IUnitOfWork unitOfWork)
        {
            _basketRepository = basketRepository;
            _logger = logger;
            _productRepository = productRepository;
            _basketItemRepository = basketItemRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> CreateBasketAsync()
        {
            var basket = new Basket();
            await _basketRepository.CreateBasketAsync(basket);
            await _unitOfWork.SaveChangesAsync();
            return basket.BasketId;
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

            await _unitOfWork.SaveChangesAsync();
        }
     

        public async Task DeleteItemFromBasketAsync( Guid basketItemId)
        {
           

            await _basketItemRepository.DeleteBasketItem(basketItemId);
            await _unitOfWork.SaveChangesAsync();
            _logger.Information("Removed item {BasketItemId} ", basketItemId);

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
            await _unitOfWork.SaveChangesAsync();
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
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveBasketItemByOne(Guid basketItemId)
        {
            var existingBasketItem = await _basketItemRepository.GetBasketItemByIdAsync(basketItemId);

            if (existingBasketItem == null)
            {
                _logger.Warning("Item with ID {BasketItemId} not found", basketItemId);
                return;
            }

            if (existingBasketItem.Quantity > 1)
            {
                existingBasketItem.Quantity -= 1;
                await _basketItemRepository.UpdateBasketItemAsync(basketItemId, existingBasketItem);
            }
            else
            {
                await _basketItemRepository.DeleteBasketItem(basketItemId);
                
            }
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
