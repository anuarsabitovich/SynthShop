using SynthShop.Domain.Entities;
using SynthShop.Domain.Extensions;


namespace SynthShop.Core.Services.Interfaces;

public interface IProductService
{
    Task CreateAsync(Product product, Stream pictureStream, string contentType, string extension);

    Task<PagedList<Product>> GetAllAsync(
        int? pageSize, int pageNumber = 1,
        string? searchTerm = null,
        string? sortBy = null, bool? isAscending = true,
        Guid? categoryId = null
    );

    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> UpdateAsync(Guid id, Product product, Stream pictureStream, string contentType, string extension);
    Task<Product?> DeleteAsync(Guid id);
}