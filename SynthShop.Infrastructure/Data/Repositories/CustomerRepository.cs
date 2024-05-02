using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Infrastructure.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {

        private readonly MainDbContext _DbContext;

        public CustomerRepository(MainDbContext context)
        {
            _DbContext = context;
        }

        public async Task<User> CreateAsync(User user)
        {
            await _DbContext.AddAsync(user);
            await _DbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User?> DeleteAsync(Guid id)
        {
            var existingCustomer = await _DbContext.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (existingCustomer == null)
            {
                return null;
            }

            existingCustomer.IsDeleted = existingCustomer.IsDeleted ? true : true;
            await _DbContext.SaveChangesAsync();
            return existingCustomer;
        }

        public Task<List<User>> GetAllAsync()
        {
            var customers = _DbContext.Customers.AsNoTracking().ToListAsync();

            return customers;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var existingCustomer = await _DbContext.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (existingCustomer == null)
            {
                return null;
            }
            return existingCustomer;
        }

        public async Task<User?> UpdateAsync(Guid id, User user)
        {
            var existingCustomer = await _DbContext.Customers.FirstOrDefaultAsync(x => x.Id == id);

            if (existingCustomer == null)
            {
                return null;
            }
            existingCustomer.FirstName = user.FirstName;
            existingCustomer.LastName = user.LastName;
            existingCustomer.Email = user.Email;
            existingCustomer.Address = user.Address;
            existingCustomer.UpdateAt = DateTime.UtcNow;
            await _DbContext.SaveChangesAsync();
            return existingCustomer;    

        }
    }
}
