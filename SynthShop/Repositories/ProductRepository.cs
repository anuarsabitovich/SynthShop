using Microsoft.EntityFrameworkCore;
using SynthShop.Data;
using SynthShop.Data.Entities;

namespace SynthShop.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MainDbContext _dbContext;

        public ProductRepository(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Product> CreateAsync(Product product)
        {
            await _dbContext.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            var products = await _dbContext.Products.AsNoTracking().ToListAsync();
            return products;
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == id);
            if (existingProduct == null)
            {
                return null;
            }
            return existingProduct;
        }

        public async Task<Product?> UpdateAsync(Guid id, Product product)
        {
            var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == id);
            if (existingProduct == null)
            {
                return null;
            }
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.StockQuantity = product.StockQuantity;
            existingProduct.CategoryID = product.CategoryID;

            return existingProduct;
        }

        public async Task<Product?> DeleteAsync(Guid id)
        {
            var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == id);
            if (existingProduct == null)
            {
                return null;
            }
            existingProduct.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return existingProduct;
        }
    }
}
