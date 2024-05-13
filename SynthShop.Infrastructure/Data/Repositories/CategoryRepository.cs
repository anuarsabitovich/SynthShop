using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Infrastructure.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger _logger;

        public CategoryRepository(MainDbContext dbContext, ILogger logger)
        {
            _dbContext = dbContext;
            _logger = logger.ForContext<CategoryRepository>();
        }

        public async Task<Category> CreateAsync(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            _logger.Information("Category created with ID {CategoryId}", category.CategoryID);
            return category;
        }

        public async Task<List<Category>> GetAllAsync(string? sortBy, bool? IsAscending = true)
        {
            var categories = _dbContext.Categories.AsQueryable();

            if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
            {
                categories = IsAscending?? true ? categories.OrderBy(x => x.Name) : categories.OrderByDescending(x => x.Name);
            }
            
            return await categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryID == id);
        }

        public async Task<Category?> UpdateAsync(Category category)
        {
            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();
            _logger.Information("Category updated with ID {CategoryId}", category.CategoryID);
            return category;
        }

        public async Task<Category?> DeleteAsync(Category category)
        {
            category.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            _logger.Information("Category marked as deleted with ID {CategoryId}", category.CategoryID);
            return category;
        }
    }
}