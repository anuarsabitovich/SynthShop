using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;
using System.Linq.Expressions;
using SynthShop.Domain.Extensions;


namespace SynthShop.Infrastructure.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly MainDbContext _dbContext;

    public ProductRepository(MainDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        await _dbContext.Products.AddAsync(product);
        return product;
    }


    public async Task<PagedList<Product>> GetAllAsync(Expression<Func<Product, bool>> filter = null,
        string? sortBy = null, bool isAscending = true,
        int pageNumber = 1, int pageSize = 1000, string? includeProperties = null,
        Guid? categoryId = null
    )
    {
        var products = _dbContext.Products.AsQueryable();

        if (includeProperties is not null)
            foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
                products = products.Include(includeProperty);

        if (categoryId.HasValue) products = products.Where(p => p.CategoryID == categoryId.Value);


        if (filter is not null) products = products.Where(filter);


        // Sorting
        if (string.IsNullOrWhiteSpace(sortBy) == false)
        {
            if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                products = isAscending ? products.OrderBy(x => x.Name) : products.OrderByDescending(x => x.Name);
            else if (sortBy.Equals("Price", StringComparison.OrdinalIgnoreCase))
                products = isAscending ? products.OrderBy(x => x.Price) : products.OrderByDescending(x => x.Price);
            else if (sortBy.Equals("StockQuantity", StringComparison.OrdinalIgnoreCase))
                products = isAscending
                    ? products.OrderBy(x => x.StockQuantity)
                    : products.OrderByDescending(x => x.StockQuantity);
        }


        return products.ToPagedList(pageNumber, pageSize);
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(x => x.ProductID == id);
    }

    public async Task<Product?> UpdateAsync(Product product)
    {
        _dbContext.Products.Update(product);
        return product;
    }

    public async Task<Product?> DeleteAsync(Product product)
    {
        product.IsDeleted = true;
        return product;
    }
}