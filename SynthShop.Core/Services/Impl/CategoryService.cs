using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Core.Services.Impl
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger _logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger.ForContext<CategoryService>();
        }

        public async Task CreateAsync(Category category)
        {
            if (category == null)
            {
                _logger.Warning("Attempted to create a null category");
                return;
            }

            var existingCategory = await _categoryRepository.GetAllAsync();
            if (existingCategory.Exists(x => x.Name.Equals(category.Name, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.Warning("Attempted to create a category with a duplicate name: {CategoryName}", category.Name);
                throw new InvalidOperationException($"Category with name '{category.Name}' already exists.");
            }

            await _categoryRepository.CreateAsync(category);
            _logger.Information("Category created with ID {CategoryId}", category.CategoryID);
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                _logger.Warning("Category with ID {CategoryId} not found", id);
                return null;
            }

            return category;
        }

        public async Task<Category?> UpdateAsync(Guid id, Category updatedCategory)
        {
            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
            {
                _logger.Warning("Category with ID {CategoryId} not found for update", id);
                return null;
            }

            existingCategory.Name = updatedCategory.Name;
            existingCategory.Description = updatedCategory.Description;
            existingCategory.UpdateAt = DateTime.UtcNow;

            var updated = await _categoryRepository.UpdateAsync(existingCategory);
            _logger.Information("Category with ID {CategoryId} updated", id);
            return updated;
        }

        public async Task<Category?> DeleteAsync(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                _logger.Warning("Category with ID {CategoryId} not found for deletion", id);
                return null;
            }

            var deletedCategory = await _categoryRepository.DeleteAsync(category);
            _logger.Information("Category with ID {CategoryId} marked as deleted", id);
            return deletedCategory;
        }
    }
}
