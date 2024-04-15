using SynthShop.Data.Entities;

namespace SynthShop.Repositories
{
    public interface IProductRepository
    {
        Task<Product> CreateAsync(Product product);
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> UpdateAsync(Guid id, Product product);
        Task<Product?> DeleteAsync(Guid id);    

    }
}
