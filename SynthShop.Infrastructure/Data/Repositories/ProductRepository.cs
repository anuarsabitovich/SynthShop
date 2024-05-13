using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Infrastructure.Data.Repositories
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
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

      

        public async Task<List<Product>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool IsAscending = true,
            int pageNumber = 1, int pageSize = 1000
            )
        {
            var products = _dbContext.Products.AsQueryable();
            
            // Filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(x => x.Name.Contains(filterQuery));
                }
            }
            
            // Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    products = IsAscending ? products.OrderBy(x => x.Name): products.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Equals("Price", StringComparison.OrdinalIgnoreCase))
                {
                    products = IsAscending ? products.OrderBy(x => x.Price) : products.OrderByDescending(x => x.Price);
                }
                else if (sortBy.Equals("StockQuantity", StringComparison.OrdinalIgnoreCase))
                {
                    products = IsAscending
                        ? products.OrderBy(x => x.StockQuantity)
                        : products.OrderByDescending(x => x.StockQuantity);
                }
            }
            
            // Pagination 
            var skipResult = (pageNumber-1) * pageSize;
            
            return await products.Skip(skipResult).Take(pageSize).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(x => x.ProductID == id);
        }

        public async Task<Product?> UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> DeleteAsync(Product product)
        {
            await _dbContext.SaveChangesAsync();
            return product;
        }
        
    }
}
