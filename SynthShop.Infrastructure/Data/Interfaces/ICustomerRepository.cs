
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Domain.Intefaces

{
    public interface ICustomerRepository
    {
        Task<Customer> CreateAsync(Customer customer);
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> UpdateAsync(Guid id, Customer customer);
        Task<Customer?> DeleteAsync(Guid id);
    }
}
