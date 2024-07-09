using SynthShop.Domain.Entities;
using LanguageExt.Common;
using SynthShop.Domain.Extensions;


namespace SynthShop.Core.Services.Interfaces;

public interface ICategoryService
{
    Task<Result<Category>> CreateAsync(Category category);

    Task<PagedList<Category>> GetAllAsync(int? pageSize, int pageNumber = 1, string? searchTerm = null,
        string? sortBy = null, bool? isAscending = true);

    Task<Category?> GetByIdAsync(Guid id);
    Task<Category?> UpdateAsync(Guid id, Category category);
    Task<Category?> DeleteAsync(Guid id);
}