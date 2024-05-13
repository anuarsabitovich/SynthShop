
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> CreateAsync(Product product);
        Task<List<Product>> GetAllAsync(string? filterOn = null, string? filterQuery = null, 
            string? sortBy = null, bool IsAscending = true,
            int pageNumber = 1, int pageSize = 1000);
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> UpdateAsync(Product product);
        Task<Product?> DeleteAsync(Product product);    

    }
}
