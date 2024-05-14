using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;
using X.PagedList;

namespace SynthShop.Core.Services.Impl
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger _logger;

        public ProductService(IProductRepository productRepository, ILogger logger)
        {
            _productRepository = productRepository;
            _logger = logger.ForContext<ProductService>();
        }

        public async Task CreateAsync(Product product)
        {
            if (product == null)
            {
                _logger.Warning("Attempted to create a null product");
                return;
            }

            await _productRepository.CreateAsync(product);
            _logger.Information("Product created with ID {ProductId}", product.ProductID);
        }

        public async Task<IPagedList<Product>> GetAllAsync( string? searchTerm = null, 
            string? sortBy = null, bool? isAscending = true,
            int pageNumber = 1, int pageSize = 1000)
        {
            Expression<Func<Product, bool>> filter = searchTerm is not null ?  x => x.Name.Contains(searchTerm) : null  ;
            //Expression<Func<User, bool>> filte = searchTerm is not null ?  x => x.FirstName.Contains(searchTerm) || x.LastName.Contains(searchTerm) || x.UserName.Contains(searchTerm) : null  ;

            return await _productRepository.GetAllAsync(filter, sortBy, isAscending ?? true, pageNumber, pageSize);
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product?> UpdateAsync(Guid id, Product updatedProduct)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                _logger.Warning("Product with ID {ProductId} not found for update", id);
                return null;
            }

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.StockQuantity = updatedProduct.StockQuantity;
            existingProduct.CategoryID = updatedProduct.CategoryID;
            existingProduct.UpdateAt = DateTime.UtcNow;

            var updated = await _productRepository.UpdateAsync(existingProduct);
            _logger.Information("Product with ID {ProductId} updated", id);
            return updated;
        }

        public async Task<Product?> DeleteAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.Warning("Product with ID {ProductId} not found for deletion", id);
                return null;
            }

            product.IsDeleted = true;
            var deletedProduct = await _productRepository.DeleteAsync(product);
            _logger.Information("Product with ID {ProductId} marked as deleted", id);
            return deletedProduct;
        }
    }
}
