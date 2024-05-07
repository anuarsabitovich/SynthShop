﻿

using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> CreateAsync(Category category);
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category?> UpdateAsync( Category category);
        Task<Category?> DeleteAsync(Category category);

    }
}
