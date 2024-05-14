﻿
using SynthShop.Domain.Entities;
using System.Linq.Expressions;
using X.PagedList;

namespace SynthShop.Infrastructure.Data.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> CreateAsync(Product product);
        Task<IPagedList<Product>> GetAllAsync(Expression<Func<Product, bool>> filter = null, 
            string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 1000, string? includeProperties = null);
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> UpdateAsync(Product product);
        Task<Product?> DeleteAsync(Product product);    

    }
}
