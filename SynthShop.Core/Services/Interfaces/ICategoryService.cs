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
    public interface ICategoryService
    {
        Task CreateAsync(Category category);
        Task<PagedList<Category>> GetAllAsync(int? pageSize, int pageNumber = 1, string? searchTerm = null,
            string? sortBy = null, bool? isAscending = true);
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category?> UpdateAsync(Guid id, Category category);
        Task<Category?> DeleteAsync(Guid id);
    }
}
