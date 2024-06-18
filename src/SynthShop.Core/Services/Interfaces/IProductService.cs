using Microsoft.AspNetCore.Mvc;
using SynthShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynthShop.Domain.Extensions;


namespace SynthShop.Core.Services.Interfaces
{
    public interface IProductService
    {
        Task CreateAsync(Product product);
        Task<PagedList<Product>> GetAllAsync(
            int? pageSize, int pageNumber = 1,
            string? searchTerm = null,
            string? sortBy = null, bool? isAscending = true,
            Guid? categoryId = null
            );
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product?> UpdateAsync(Guid id, Product product);
        Task<Product?> DeleteAsync(Guid id);
    }
}
