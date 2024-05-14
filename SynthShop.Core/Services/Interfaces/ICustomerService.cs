using SynthShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynthShop.Domain.Extensions;


namespace SynthShop.Core.Services.Interfaces
{
    public interface ICustomerService
    {
        Task CreateAsync(User user);
        Task<PagedList<User>> GetAllAsync( string? searchTerm,
            string? sortBy = null, bool? IsAscending = true,
            int pageNumber = 1, int pageSize = 1000);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> UpdateAsync(Guid id, User user);
        Task<User?> DeleteAsync(Guid id);
    }
}
