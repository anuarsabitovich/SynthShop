

using SynthShop.Domain.Entities;
using System.Linq.Expressions;
using X.PagedList;

namespace SynthShop.Infrastructure.Data.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> CreateAsync(Category category);
        Task<IPagedList<Category>> GetAllAsync(Expression<Func<Category, bool>> filter = null,
            string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 1000, string? includeProperties = null);
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category?> UpdateAsync( Category category);
        Task<Category?> DeleteAsync(Category category);

    }
}
