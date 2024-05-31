using System.Linq.Expressions;
using LanguageExt.Common;
using Microsoft.Extensions.Options;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Exceptions;
using SynthShop.Domain.Extensions;
using SynthShop.Domain.Settings;
using SynthShop.Infrastructure.Data.Interfaces;


namespace SynthShop.Core.Services.Impl
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly PagingSettings _pagingSettings;
        private readonly ILogger _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository categoryRepository, ILogger logger, IOptions<PagingSettings> pagingSettings, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _pagingSettings = pagingSettings.Value;
            _logger = logger.ForContext<CategoryService>();
        }

        public async Task<Result<Category>> CreateAsync(Category category)
        {
            Expression<Func<Category, bool>> filter = x => x.Name.ToLower().Contains(category.Name.ToLower());

            var existingCategory = await _categoryRepository.GetAllAsync(filter);
            if (existingCategory.Items.Any())
            {
                _logger.Warning("Attempted to create a category with a duplicate name: {CategoryName}", category.Name);
                return new Result<Category>(
                    new CategoryCreateFailedException($"Category with name '{category.Name}' already exists."));
            }

            await _categoryRepository.CreateAsync(category);
            await _unitOfWork.SaveChangesAsync();
            _logger.Information("Category created with ID {CategoryId}", category.CategoryID);
            return category;
        }

        public async Task<PagedList<Category>> GetAllAsync(int? pageSize, int pageNumber = 1, string? searchTerm = null,
            string? sortBy = null, bool? isAscending = true)
        {
            Expression<Func<Category, bool>> filter = searchTerm is not null ? x => x.Name.Contains(searchTerm) : null;

            return await _categoryRepository.GetAllAsync(filter, sortBy, isAscending ?? true, pageNumber, pageSize ?? _pagingSettings.PageSize );
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _categoryRepository.GetByIdAsync(id);
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
            await _unitOfWork.SaveChangesAsync();
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
            await _unitOfWork.SaveChangesAsync();
            return deletedCategory;
        }
    }
}
