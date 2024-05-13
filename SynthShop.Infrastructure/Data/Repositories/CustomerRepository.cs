using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Infrastructure.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly MainDbContext _dbContext;

        public CustomerRepository(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateAsync(User user)
        {
            await _dbContext.Customers.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool IsAscending = true,
            int pageNumber = 1, int pageSize = 1000)
        {

            var customers = _dbContext.Customers.AsQueryable();

            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    customers = customers.Where(x => x.FirstName.Contains(filterQuery));
                }
                else if (filterOn.Equals("Last Name", StringComparison.OrdinalIgnoreCase))
                {
                    customers = customers.Where(x => x.LastName.Contains(filterQuery));
                }
            }

            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    customers = IsAscending
                        ? customers.OrderBy(x => x.FirstName)
                        : customers.OrderByDescending(x => x.FirstName);
                } 
                else if (sortBy.Equals("Last Name", StringComparison.OrdinalIgnoreCase))
                {
                    customers = IsAscending
                        ? customers.OrderBy(x => x.FirstName)
                        : customers.OrderByDescending(x => x.LastName);
                }
            }
            
            var skipResult = (pageNumber - 1) * pageSize;

            return await customers.Skip(skipResult).Take(pageSize).ToListAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> UpdateAsync(User user)
        {
            _dbContext.Customers.Update(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> DeleteAsync(User user)
        {
            user.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return user;
        }
    }


}
