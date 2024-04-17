using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data;
using SynthShop.Infrastructure.Domain.Intefaces;

namespace SynthShop.Infrastructure.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {

        private readonly MainDbContext _DbContext;

        public CustomerRepository(MainDbContext context)
        {
            _DbContext = context;
        }

        public async Task<Customer> CreateAsync(Customer customer)
        {
            await _DbContext.AddAsync(customer);
            await _DbContext.SaveChangesAsync();

            return customer;
        }

        public async Task<Customer?> DeleteAsync(Guid id)
        {
            var existingCustomer = await _DbContext.Customers.FirstOrDefaultAsync(x => x.CustomerID == id);
            if (existingCustomer == null)
            {
                return null;
            }

            existingCustomer.IsDeleted = existingCustomer.IsDeleted ? true : true;
            await _DbContext.SaveChangesAsync();
            return existingCustomer;
        }

        public Task<List<Customer>> GetAllAsync()
        {
            var customers = _DbContext.Customers.AsNoTracking().ToListAsync();

            return customers;
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            var existingCustomer = await _DbContext.Customers.FirstOrDefaultAsync(x => x.CustomerID == id);
            if (existingCustomer == null)
            {
                return null;
            }
            return existingCustomer;
        }

        public async Task<Customer?> UpdateAsync(Guid id, Customer customer)
        {
            var existingCustomer = await _DbContext.Customers.FirstOrDefaultAsync(x => x.CustomerID == id);

            if (existingCustomer == null)
            {
                return null;
            }
            existingCustomer.FirstName = customer.FirstName;
            existingCustomer.LastName = customer.LastName;
            existingCustomer.Email = customer.Email;
            existingCustomer.Address = customer.Address;
            existingCustomer.UpdateAt = DateTime.UtcNow;
            await _DbContext.SaveChangesAsync();
            return existingCustomer;    

        }
    }
}
