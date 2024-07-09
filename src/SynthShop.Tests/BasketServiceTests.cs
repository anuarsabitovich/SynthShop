using NSubstitute;
using Serilog;
using SynthShop.Core.Services.Impl;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Tests;

public class BasketServiceTests
{
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger _logger;
    private readonly IProductRepository _productRepository;
    private readonly IBasketItemRepository _basketItemRepository;
    private readonly BasketService _sut;
    private readonly IUnitOfWork _unitOfWork;

    public BasketServiceTests()
    {
        _basketRepository = Substitute.For<IBasketRepository>();
        _logger = Substitute.For<ILogger>();
        _productRepository = Substitute.For<IProductRepository>();
        _basketItemRepository = Substitute.For<IBasketItemRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _sut = new BasketService(_basketRepository, _logger, _productRepository, _basketItemRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateBasketAsync_ShouldReturnBasketId()
    {
        // Arrange
        var basketId = Guid.NewGuid();

        _basketRepository.When(x => x.CreateBasketAsync(Arg.Any<Basket>()))
            .Do(x => x.Arg<Basket>().BasketId = basketId);
        _unitOfWork.SaveChangesAsync().Returns(Task.CompletedTask);

        // Act
        var result = await _sut.CreateBasketAsync();

        // Assert
        Assert.Equal(basketId, result);
        await _basketRepository.Received(1).CreateBasketAsync(Arg.Is<Basket>(b => b.BasketId == basketId));
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetBasketByIdAsnyc_ShouldReturnBasket_WhenBasketExists()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basket = new Basket() { BasketId = basketId };
        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));

        // Act 

        var result = await _sut.GetBasketByIdAsync(basketId);

        // Assert

        Assert.Equal(basket, result);
    }

    [Fact]
    public async Task GetBasketByIdAsync_ShouldReturnNull_WhenBasketDoesntExists()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult<Basket>(null));

        // Act 

        var result = await _sut.GetBasketByIdAsync(basketId);

        // Assert

        Assert.Null(result);
    }

    [Fact]
    public async Task AddItemToBasketAsync_ShouldAddNewItem_WhenItemDoesNotExistInBasket()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantity = 1;
        var basket = new Basket() { BasketId = basketId };

        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));
        _productRepository.GetByIdAsync(productId)
            .Returns(Task.FromResult(new Product() { ProductID = productId }));

        // Act
        await _sut.AddItemToBasketAsync(basketId, productId, quantity);

        // Assert
        await _basketItemRepository.Received(1).CreateBasketItemAsync(
            Arg.Is<BasketItem>(item =>
                item.BasketId == basketId && item.ProductId == productId && item.Quantity == quantity));
    }

    [Fact]
    public async Task AddItemToBasketAsync_ShouldUpdateQuantity_WhenItemExistInBasket()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var initialQuantity = 1;
        var additionalQuantity = 2;
        var basket = new Basket
        {
            BasketId = basketId,
            Items = new List<BasketItem>
            {
                new()
                {
                    BasketItemId = Guid.NewGuid(), BasketId = basketId, ProductId = productId,
                    Quantity = initialQuantity
                }
            }
        };

        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));
        _productRepository.GetByIdAsync(productId).Returns(Task.FromResult(new Product { ProductID = productId }));


        // Act

        await _sut.AddItemToBasketAsync(basketId, productId, additionalQuantity);

        // Assert
        var updatedItem = basket.Items.First();
        await _basketItemRepository.Received(1).UpdateBasketItemAsync(updatedItem.BasketItemId,
            Arg.Is<BasketItem>(item => item.Quantity == initialQuantity + additionalQuantity));
    }

    [Fact]
    public async Task DeleteItemFromBasketAsync_ShouldRemoveItem_WhenItemExists()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basketItemId = Guid.NewGuid();
        var basket = new Basket
        {
            BasketId = basketId,
            Items = new List<BasketItem>
            {
                new() { BasketItemId = basketItemId, BasketId = basketId, ProductId = Guid.NewGuid(), Quantity = 1 }
            }
        };

        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));

        // Act
        await _sut.DeleteItemFromBasketAsync(basketId, basketItemId);

        // Assert
        await _basketItemRepository.Received(1).DeleteBasketItem(basketItemId);
        _logger.Received(1).Information("Removed item {BasketItemId} from basket {BasketId}", basketItemId, basketId);
    }

    [Fact]
    public async Task DeleteItemFromBasketAsync_ShouldLogWarning_WhenItemDoesNotExist()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basketItemId = Guid.NewGuid();
        var basket = new Basket
        {
            BasketId = basketId,
            Items = new List<BasketItem>()
        };

        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));

        // Act
        await _sut.DeleteItemFromBasketAsync(basketId, basketItemId);

        // Assert
        await _basketItemRepository.DidNotReceive().DeleteBasketItem(Arg.Any<Guid>());
        _logger.Received(1).Warning("Item with ID {BasketItemId} not found in basket {BasketId}", basketItemId,
            basketId);
    }

    [Fact]
    public async Task DeleteBasketAsync_ShouldDeleteBasket_WhenBasketExists()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basket = new Basket { BasketId = basketId };
        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));

        // Act
        await _sut.DeleteBasketAsync(basketId);

        // Assert
        await _basketRepository.Received(1).DeleteBasketAsync(basketId);
    }

    [Fact]
    public async Task DeleteBasketAsync_ShouldLogWarning_WhenBasketDoesNotExist()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult<Basket>(null));

        // Act
        await _sut.DeleteBasketAsync(basketId);

        // Assert
        await _basketRepository.DidNotReceive().DeleteBasketAsync(Arg.Any<Guid>());
        _logger.Received(1).Warning("Basket {BasketId} not found", basketId);
    }

    [Fact]
    public async Task UpdateItemInBasket_ShouldUpdateItem_WhenItemExists()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basketItemId = Guid.NewGuid();
        var newQuantity = 5;
        var basket = new Basket
        {
            BasketId = basketId,
            Items = new List<BasketItem>
            {
                new() { BasketItemId = basketItemId, BasketId = basketId, ProductId = Guid.NewGuid(), Quantity = 1 }
            }
        };

        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));

        // Act
        await _sut.UpdateItemInBasket(basketId, basketItemId, newQuantity);

        // Assert
        await _basketItemRepository.Received(1)
            .UpdateBasketItemAsync(basketItemId, Arg.Is<BasketItem>(item => item.Quantity == newQuantity));
    }

    [Fact]
    public async Task UpdateItemInBasket_ShouldLogWarning_WhenItemDoesNotExist()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basketItemId = Guid.NewGuid();
        var newQuantity = 5;
        var basket = new Basket
        {
            BasketId = basketId,
            Items = new List<BasketItem>()
        };

        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));

        // Act
        await _sut.UpdateItemInBasket(basketId, basketItemId, newQuantity);

        // Assert
        await _basketItemRepository.DidNotReceive().UpdateBasketItemAsync(Arg.Any<Guid>(), Arg.Any<BasketItem>());
        _logger.Received(1).Warning("Item with ID {BasketItemId} not found in basket {BasketId}", basketItemId,
            basketId);
    }
}