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
    public interface ICategoryService
    {
        Task CreateAsync(Category category);
        Task<IPagedList<Category>> GetAllAsync(string? searchTerm = null,
            string? sortBy = null, bool? isAscending = true,
            int pageNumber = 1, int pageSize = 1000);
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category?> UpdateAsync(Guid id, Category category);
        Task<Category?> DeleteAsync(Guid id);
    }
}
