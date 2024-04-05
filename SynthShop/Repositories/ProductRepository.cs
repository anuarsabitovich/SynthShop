using Microsoft.EntityFrameworkCore;
using SynthShop.Data;
using SynthShop.Data.Entities;

namespace SynthShop.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MainDbContext dbContext;

        public ProductRepository(MainDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> DeleteAsync(int id)
        {
            var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (existingProduct == null)
            {
                return null;
            }
            dbContext.Products.Remove(existingProduct);
            await dbContext.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await dbContext.Products.ToListAsync();
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            return dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Product?> UpdateAsync(int id, Product product)
        {
            var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (existingProduct == null)
            {
                return null;
            }
           
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price  = product.Price;
            existingProduct.QuantityInStock = product.QuantityInStock;

            await dbContext.SaveChangesAsync();
            return existingProduct;
        }
    }
}
