using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using Serilog;
using SynthShop.Core.Services.Impl;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;
using Xunit;

namespace SynthShop.Tests;

public class BasketServiceTests
{
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger _logger;
    private readonly IProductRepository _productRepository;
    private readonly IBasketItemRepository _basketItemRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly BasketService _sut;

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
    public async Task CreateBasketAsync_ShouldCreateBasketAndReturnBasketId()
    {
        // Arrange
        _basketRepository.CreateBasketAsync(Arg.Do<Basket>(x => x.BasketId = Guid.NewGuid())).Returns(Task.CompletedTask);
        _unitOfWork.SaveChangesAsync().Returns(Task.CompletedTask);

        // Act
        var basketId = await _sut.CreateBasketAsync();

        // Assert
        await _basketRepository.Received(1).CreateBasketAsync(Arg.Any<Basket>());
        await _unitOfWork.Received(1).SaveChangesAsync();
        Assert.NotEqual(Guid.Empty, basketId);
    }

    [Fact]
    public async Task GetBasketByIdAsync_ShouldReturnBasket_WhenBasketExists()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var expectedBasket = new Basket { BasketId = basketId };
        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(expectedBasket));

        // Act
        var basket = await _sut.GetBasketByIdAsync(basketId);

        // Assert
        Assert.NotNull(basket);
        Assert.Equal(basketId, basket.BasketId);
    }

    [Fact]
    public async Task GetBasketByIdAsync_ShouldReturnNull_WhenBasketDoesNotExist()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult<Basket>(null));

        // Act
        var basket = await _sut.GetBasketByIdAsync(basketId);

        // Assert
        Assert.Null(basket);
    }

    [Fact]
    public async Task AddItemToBasketAsync_ShouldAddNewItem_WhenProductExistsAndBasketExists()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product { ProductID = productId };
        var basket = new Basket { BasketId = basketId, Items = new List<BasketItem>() };

        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));
        _productRepository.GetByIdAsync(productId).Returns(Task.FromResult(product));

        // Act
        await _sut.AddItemToBasketAsync(basketId, productId, 1);

        // Assert
        await _basketItemRepository.Received(1).CreateBasketItemAsync(Arg.Any<BasketItem>());
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task AddItemToBasketAsync_ShouldUpdateExistingItem_WhenProductExistsInBasket()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product { ProductID = productId };
        var existingItem = new BasketItem { BasketId = basketId, ProductId = productId, Quantity = 1 };
        var basket = new Basket { BasketId = basketId, Items = new List<BasketItem> { existingItem } };

        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));
        _productRepository.GetByIdAsync(productId).Returns(Task.FromResult(product));

        // Act
        await _sut.AddItemToBasketAsync(basketId, productId, 1);

        // Assert
        await _basketItemRepository.Received(1).UpdateBasketItemAsync(existingItem.BasketItemId, existingItem);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteItemFromBasketAsync_ShouldDeleteItem_WhenItemExists()
    {
        // Arrange
        var basketItemId = Guid.NewGuid();

        // Act
        await _sut.DeleteItemFromBasketAsync(basketItemId);

        // Assert
        await _basketItemRepository.Received(1).DeleteBasketItem(basketItemId);
        await _unitOfWork.Received(1).SaveChangesAsync();
        _logger.Received(1).Information("Removed item {BasketItemId} ", basketItemId);
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
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateItemInBasket_ShouldUpdateQuantity_WhenItemExists()
    {
        // Arrange
        var basketId = Guid.NewGuid();
        var basketItemId = Guid.NewGuid();
        var existingItem = new BasketItem { BasketItemId = basketItemId, Quantity = 1 };
        var basket = new Basket { BasketId = basketId, Items = new List<BasketItem> { existingItem } };

        _basketRepository.GetBasketByIdAsync(basketId).Returns(Task.FromResult(basket));

        // Act
        await _sut.UpdateItemInBasket(basketId, basketItemId, 5);

        // Assert
        await _basketItemRepository.Received(1).UpdateBasketItemAsync(basketItemId, existingItem);
        await _unitOfWork.Received(1).SaveChangesAsync();
        Assert.Equal(5, existingItem.Quantity);
    }

    [Fact]
    public async Task RemoveBasketItemByOne_ShouldDecreaseQuantity_WhenQuantityGreaterThanOne()
    {
        // Arrange
        var basketItemId = Guid.NewGuid();
        var existingItem = new BasketItem { BasketItemId = basketItemId, Quantity = 5 };

        _basketItemRepository.GetBasketItemByIdAsync(basketItemId).Returns(Task.FromResult(existingItem));

        // Act
        await _sut.RemoveBasketItemByOne(basketItemId);

        // Assert
        await _basketItemRepository.Received(1).UpdateBasketItemAsync(basketItemId, existingItem);
        await _unitOfWork.Received(1).SaveChangesAsync();
        Assert.Equal(4, existingItem.Quantity);
    }

    [Fact]
    public async Task RemoveBasketItemByOne_ShouldDeleteItem_WhenQuantityIsOne()
    {
        // Arrange
        var basketItemId = Guid.NewGuid();
        var existingItem = new BasketItem { BasketItemId = basketItemId, Quantity = 1 };

        _basketItemRepository.GetBasketItemByIdAsync(basketItemId).Returns(Task.FromResult(existingItem));

        // Act
        await _sut.RemoveBasketItemByOne(basketItemId);

        // Assert
        await _basketItemRepository.Received(1).DeleteBasketItem(basketItemId);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}
