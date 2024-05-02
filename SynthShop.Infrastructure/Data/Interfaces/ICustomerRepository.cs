
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Interfaces

{
    public interface ICustomerRepository
    {
        Task<User> CreateAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> UpdateAsync(Guid id, User user);
        Task<User?> DeleteAsync(Guid id);
    }
}
