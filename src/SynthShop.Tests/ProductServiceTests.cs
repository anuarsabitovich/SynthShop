using System;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NSubstitute;
using Serilog;
using SynthShop.Core.Services.Impl;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Extensions;
using SynthShop.Domain.Settings;
using SynthShop.Infrastructure.Data.Interfaces;
using Xunit;

namespace SynthShop.Tests
{
    public class ProductServiceTests
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStorageService _storageService;
        private readonly IOptions<PagingSettings> _pagingSettings;
        private readonly IOptions<AWSSettings> _awsSettings;
        private readonly ProductService _sut;

        public ProductServiceTests()
        {
            _productRepository = Substitute.For<IProductRepository>();
            _logger = Substitute.For<ILogger>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _storageService = Substitute.For<IStorageService>();
            _pagingSettings = Options.Create(new PagingSettings { PageSize = 10 });
            _awsSettings = Options.Create(new AWSSettings
            {
                CloudFrontDomainUrl = "https://example.cloudfront.net",
                BucketName = "example-bucket"
            });
            _sut = new ProductService(_productRepository, _logger, _pagingSettings, _unitOfWork, _storageService, _awsSettings);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateProductAndUploadPicture()
        {
            // Arrange
            var product = new Product { Name = "Test Product" };
            var pictureStream = new MemoryStream();
            var contentType = "image/jpeg";
            var extension = "jpg";

            _productRepository.CreateAsync(Arg.Any<Product>()).Returns(ci =>
            {
                var createdProduct = ci.Arg<Product>();
                createdProduct.ProductID = Guid.NewGuid(); 
                return Task.FromResult(createdProduct);
            });

            // Act
        
            await _sut.CreateAsync(product, pictureStream, contentType, extension);
             
          
            // Assert
            await _storageService.Received(1).UploadAsync(Arg.Any<string>(), pictureStream, contentType);
            await _productRepository.Received(1).CreateAsync(Arg.Any<Product>());
            await _unitOfWork.Received(1).SaveChangesAsync();
        }






        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedProducts()
        {
            // Arrange
            var pagedList = new PagedList<Product>(new List<Product> { new Product { Name = "Test Product" } }, 1, 1, 1);
            _productRepository.GetAllAsync(Arg.Any<Expression<Func<Product, bool>>>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<Guid?>())
                .Returns(Task.FromResult(pagedList));

            // Act
            var result = await _sut.GetAllAsync(null, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
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
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductID);
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
        public async Task UpdateAsync_ShouldUpdateProductAndUploadNewPicture()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var existingProduct = new Product { ProductID = productId, Name = "Old Product" };
            var updatedProduct = new Product { Name = "Updated Product", Description = "Updated Description", Price = 100, StockQuantity = 10, CategoryID = Guid.NewGuid() };
            var pictureStream = new MemoryStream();
            var contentType = "image/jpeg";
            var extension = "jpg";

            _productRepository.GetByIdAsync(productId).Returns(Task.FromResult(existingProduct));
            _productRepository.UpdateAsync(existingProduct).Returns(Task.FromResult(existingProduct));

            // Act
            var result = await _sut.UpdateAsync(productId, updatedProduct, pictureStream, contentType, extension);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Product", result.Name);
            await _storageService.Received(1).UploadAsync(Arg.Any<string>(), pictureStream, contentType);
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updatedProduct = new Product { Name = "Updated Product" };
            var pictureStream = new MemoryStream();
            var contentType = "image/jpeg";
            var extension = "jpg";

            _productRepository.GetByIdAsync(productId).Returns(Task.FromResult<Product>(null));

            // Act
            var result = await _sut.UpdateAsync(productId, updatedProduct, pictureStream, contentType, extension);

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
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _productRepository.GetByIdAsync(productId).Returns(Task.FromResult<Product>(null));

            // Act
            var result = await _sut.DeleteAsync(productId);

            // Assert
            Assert.Null(result);
        }
    }
}
