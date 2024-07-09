using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Extensions;
using SynthShop.Infrastructure.Data.Interfaces;


namespace SynthShop.Infrastructure.Data.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly MainDbContext _dbContext;

    public CategoryRepository(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Category> CreateAsync(Category category)
    {
        await _dbContext.Categories.AddAsync(category);
        return category;
    }

    public async Task<PagedList<Category>> GetAllAsync(Expression<Func<Category, bool>> filter = null,
        string? sortBy = null, bool isAscending = true,
        int pageNumber = 1, int pageSize = 1000, string? includeProperties = null)
    {
        var categories = _dbContext.Categories.AsQueryable();

        if (includeProperties is not null)
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
                categories = categories.Include(includeProperty);

        if (filter is not null) categories = categories.Where(filter);

        if (string.IsNullOrWhiteSpace(sortBy) == false)
            if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                categories = isAscending
                    ? categories.OrderBy(x => x.Name)
                    : categories.OrderByDescending(x => x.Name);

        return categories.ToPagedList(pageNumber, pageSize);
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryID == id);
    }

    public async Task<Category?> UpdateAsync(Category category)
    {
        _dbContext.Categories.Update(category);
        return category;
    }

    public async Task<Category?> DeleteAsync(Category category)
    {
        category.IsDeleted = true;
        return category;
    }
}