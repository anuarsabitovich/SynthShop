using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Seed
{
    public class CategorySeeder:ISeeder
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger _logger;

        public CategorySeeder(MainDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            var path = Utils.GetSeedConfigPath("Categories.json");
           
            if (await _dbContext.Categories.AnyAsync())
            {
                _logger.Information("Db has categories");
                return;
            }

            var categoryData =  await File.ReadAllTextAsync(path);
            var categories = JsonSerializer.Deserialize<List<Category>>(categoryData);
           
            foreach (var category in categories)
            {
                category.IsDeleted = false;
                category.CreatedAt = DateTime.UtcNow;
            }

            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();
        }
    }
}
