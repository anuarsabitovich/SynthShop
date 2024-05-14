using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Extensions;
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

        public async Task<PagedList<Category>> GetAllAsync(Expression<Func<Category, bool>> filter = null,
            string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 1000, string? includeProperties = null)
        {
            var categories = _dbContext.Categories.AsQueryable();

            if (includeProperties is not null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    categories = categories.Include(includeProperty);
                }
            }

            if (filter is not null)
            {
                categories = categories.Where(filter);
            }

            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    categories = isAscending
                        ? categories.OrderBy(x => x.Name)
                        : categories.OrderByDescending(x => x.Name);
                }
            }
            
            return categories.ToPagedList(pageNumber, pageSize);
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