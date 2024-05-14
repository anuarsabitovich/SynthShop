
using SynthShop.Domain.Entities;
using System.Linq.Expressions;
using SynthShop.Domain.Extensions;


namespace SynthShop.Infrastructure.Data.Interfaces

{
    public interface ICustomerRepository
    {
        Task<User> CreateAsync(User user);
        Task<PagedList<User>> GetAllAsync(Expression<Func<User, bool>>? filter = null,
            string? sortBy = null, bool IsAscending = true,
            int pageNumber = 1, int pageSize = 1000, string? includeProperties = null);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> UpdateAsync( User user);
        Task<User?> DeleteAsync(User user);
    }
}
