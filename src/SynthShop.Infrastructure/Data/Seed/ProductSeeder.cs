using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SynthShop.Infrastructure.Data.Seed
{
    public class ProductSeeder: ISeeder
    {
        private readonly MainDbContext _dbContext;

        public ProductSeeder(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task SeedAsync()
        {
            var path = Utils.GetSeedConfigPath("Products.json");

            if (await _dbContext.Products.AnyAsync())
            {
                return;
            }

            var productData = await File.ReadAllTextAsync(path);
            var products = JsonSerializer.Deserialize<List<Product>>(productData);

            foreach (var product in products)
            {
                product.IsDeleted = false;
                product.CreatedAt = DateTime.UtcNow;
            }

            await _dbContext.Products.AddRangeAsync(products);
            await _dbContext.SaveChangesAsync();
        }
    }
}
