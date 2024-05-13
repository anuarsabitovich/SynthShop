
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Interfaces

{
    public interface ICustomerRepository
    {
        Task<User> CreateAsync(User user);
        Task<List<User>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool IsAscending = true,
            int pageNumber = 1, int pageSize = 1000);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> UpdateAsync( User user);
        Task<User?> DeleteAsync(User user);
    }
}
