using Microsoft.EntityFrameworkCore;
using SynthShop.Data;
using SynthShop.Data.Entities;

namespace SynthShop.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MainDbContext _dbContext;

        public CategoryRepository(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Category> CreateAsync(Category category)
        {
            await _dbContext.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }
        public Task<List<Category>> GetAllAsync()
        {

            var categories = _dbContext.Categories.AsNoTracking().ToListAsync();

            return categories;
        }

        public async Task<Category?> DeleteAsync(Guid id)
        {
            var existingCategory = await _dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryID == id);
            if (existingCategory == null)
            {
                return null;
            }

            existingCategory.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return existingCategory;
        }

       

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            var existingCategory = await _dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryID == id);
            if (existingCategory == null)
            {
                return null;
            }
            return existingCategory;
        }

        public async Task<Category?> UpdateAsync(Guid id, Category category)
        {
            var existingCategory = await _dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryID == id);

            if(existingCategory == null)
            {
                return null;
            }
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;

            await _dbContext.SaveChangesAsync();
            return category;
        }

      
    }
}
