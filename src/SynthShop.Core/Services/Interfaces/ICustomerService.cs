using SynthShop.Domain.Entities;
using SynthShop.Domain.Extensions;


namespace SynthShop.Core.Services.Interfaces;

public interface ICustomerService
{
    Task CreateAsync(User user);

    Task<PagedList<User>> GetAllAsync(int? pageSize, int pageNumber = 1, string? searchTerm = null,
        string? sortBy = null, bool? IsAscending = true);

    Task<User?> GetByIdAsync(Guid id);
    Task<User?> UpdateAsync(Guid id, User user);
    Task<User?> DeleteAsync(Guid id);
}