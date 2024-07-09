using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Serilog;
using SynthShop.Core.Services.Impl;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Extensions;
using SynthShop.Domain.Settings;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Tests;

public class ProductServiceTests
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger _logger;
    private readonly IOptions<PagingSettings> _pagingSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _logger = Substitute.For<ILogger>();
        _pagingSettings = Options.Create(new PagingSettings { PageSize = 10 });
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _sut = new ProductService(_productRepository, _logger, _pagingSettings, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_ShouldLogWarning_WhenProductIsNull()
    {
        // Act
        await _sut.CreateAsync(null);

        // Assert
        await _productRepository.DidNotReceive().CreateAsync(Arg.Any<Product>());
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct_WhenProductIsValid()
    {
        // Arrange
        var product = new Product { ProductID = Guid.NewGuid(), Name = "Product1" };

        // Act
        await _sut.CreateAsync(product);

        // Assert
        await _productRepository.Received(1).CreateAsync(product);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedListOfProducts()
    {
        // Arrange
        var products = new PagedList<Product>(new List<Product> { new() { Name = "Product1" } }, 1, 1, 10);
        _productRepository
            .GetAllAsync(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<int>(),
                Arg.Any<int>()).Returns(Task.FromResult(products));

        // Act
        var result = await _sut.GetAllAsync(10, 1, "Product", "Name", true);

        // Assert
        Assert.Equal(products, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { ProductID = productId };
        _productRepository.GetByIdAsync(productId).Returns(Task.FromResult(product));

        // Act
        var result = await _sut.GetByIdAsync(productId);

        // Assert
        Assert.Equal(product, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productRepository.GetByIdAsync(productId).Returns(Task.FromResult<Product>(null));

        // Act
        var result = await _sut.GetByIdAsync(productId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldLogWarningAndReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var updatedProduct = new Product { ProductID = productId, Name = "UpdatedProduct" };
        _productRepository.GetByIdAsync(productId).Returns(Task.FromResult<Product>(null));

        // Act
        var result = await _sut.UpdateAsync(productId, updatedProduct);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var existingProduct = new Product { ProductID = productId, Name = "ExistingProduct" };
        var updatedProduct = new Product
        {
            Name = "UpdatedProduct", Description = "UpdatedDescription", Price = 100, StockQuantity = 50,
            CategoryID = Guid.NewGuid()
        };
        _productRepository.GetByIdAsync(productId).Returns(Task.FromResult(existingProduct));
        _productRepository.UpdateAsync(Arg.Any<Product>()).Returns(Task.FromResult(updatedProduct));

        // Act
        var result = await _sut.UpdateAsync(productId, updatedProduct);

        // Assert
        Assert.Equal(updatedProduct.Name, existingProduct.Name);
        Assert.Equal(updatedProduct.Description, existingProduct.Description);
        Assert.Equal(updatedProduct.Price, existingProduct.Price);
        Assert.Equal(updatedProduct.StockQuantity, existingProduct.StockQuantity);
        Assert.Equal(updatedProduct.CategoryID, existingProduct.CategoryID);
        await _productRepository.Received(1).UpdateAsync(existingProduct);
    }

    [Fact]
    public async Task DeleteAsync_ShouldLogWarningAndReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productRepository.GetByIdAsync(productId).Returns(Task.FromResult<Product>(null));

        // Act
        var result = await _sut.DeleteAsync(productId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldMarkProductAsDeleted_WhenProductExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product { ProductID = productId };
        _productRepository.GetByIdAsync(productId).Returns(Task.FromResult(product));
        _productRepository.DeleteAsync(product).Returns(Task.FromResult(product));

        // Act
        var result = await _sut.DeleteAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsDeleted);
        await _productRepository.Received(1).DeleteAsync(product);
    }
}