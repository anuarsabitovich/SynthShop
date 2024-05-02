using SynthShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynthShop.Core.Services.Interfaces
{
    public interface ICustomerService
    {
        Task CreateAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> UpdateAsync(Guid id, User user);
        Task<User?> DeleteAsync(Guid id);
    }
}
