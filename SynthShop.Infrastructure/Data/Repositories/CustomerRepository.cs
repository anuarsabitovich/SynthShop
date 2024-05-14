using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data;
using SynthShop.Infrastructure.Data.Interfaces;
using X.PagedList;

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

        public async Task<IPagedList<User>> GetAllAsync(Expression<Func<User, bool>>? filter = null,
            string? sortBy = null, bool IsAscending = true,
            int pageNumber = 1, int pageSize = 1000,
            string? includeProperties = null)
        {

            var customers = _dbContext.Customers.AsQueryable();

            if (includeProperties is not null)
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    customers = customers.Include(includeProperty);
                }
            }

            if (filter is not null)
            {
                customers = customers.Where(filter);
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
            
            return await customers.ToPagedListAsync(pageNumber, pageSize);
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
