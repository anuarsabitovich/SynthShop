﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;

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

        public async Task<List<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                _logger.Warning("Product with ID {ProductId} not found", id);
                return null;
            }

            return product;
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
