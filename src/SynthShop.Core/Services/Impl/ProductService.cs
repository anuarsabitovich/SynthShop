﻿using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Extensions;
using SynthShop.Domain.Settings;
using SynthShop.Infrastructure.Data.Interfaces;


namespace SynthShop.Core.Services.Impl;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger _logger;
    private readonly PagingSettings _pagingSettings;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStorageService _storageService;
    private readonly AWSSettings _awsSettings;

    public ProductService(IProductRepository productRepository, ILogger logger, IOptions<PagingSettings> pagingSettings,
        IUnitOfWork unitOfWork, IStorageService storageService, IOptions<AWSSettings> awsSettings)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _storageService = storageService;
        _awsSettings = awsSettings.Value;
        _logger = logger.ForContext<ProductService>();
        _pagingSettings = pagingSettings.Value;
    }

    public async Task CreateAsync(Product product, Stream pictureStream, string contentType, string extension)
    {
        var fileName = $"{Guid.NewGuid()}_{product.Name}.{extension}";
        await _storageService.UploadAsync(fileName, pictureStream, contentType);
        product.PictureUrl = $"{_awsSettings.CloudFrontDomainUrl}/{fileName}";
        await _productRepository.CreateAsync(product);
        await _unitOfWork.SaveChangesAsync();
        _logger.Information("Product created with ID {ProductId}", product.ProductID);
    }

    public async Task<PagedList<Product>> GetAllAsync(int? pageSize, int pageNumber = 1, string? searchTerm = null,
        string? sortBy = null, bool? isAscending = true, Guid? categoryId = null)
    {
        Expression<Func<Product, bool>> filter = searchTerm is not null ? x => x.Name.Contains(searchTerm) : null;

        return await _productRepository.GetAllAsync(filter, sortBy, isAscending ?? true, pageNumber,
            pageSize ?? _pagingSettings.PageSize, "Category", categoryId);
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<Product?> UpdateAsync(Guid id, Product updatedProduct, Stream pictureStream, string contentType,
        string extension)
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
        var fileName = $"{Guid.NewGuid()}_{existingProduct.Name}.{extension}";
        await _storageService.UploadAsync(fileName, pictureStream, contentType);
        existingProduct.PictureUrl = $"{_awsSettings.CloudFrontDomainUrl}/{fileName}";
        var updated = await _productRepository.UpdateAsync(existingProduct);

        await _unitOfWork.SaveChangesAsync();
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
        await _unitOfWork.SaveChangesAsync();
        _logger.Information("Product with ID {ProductId} marked as deleted", id);
        return deletedProduct;
    }
}