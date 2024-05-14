using Microsoft.AspNetCore.Mvc;
using SynthShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace SynthShop.Core.Services.Interfaces
{
    public interface IProductService
    {
        Task CreateAsync(Product product);
        Task<IPagedList<Product>> GetAllAsync(
            string? searchTerm = null,
            string? sortBy = null, bool? isAscending = true,
            int pageNumber = 1, int pageSize = 1000
            );
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> UpdateAsync(Guid id, Product product);
        Task<Product?> DeleteAsync(Guid id);
    }
}
