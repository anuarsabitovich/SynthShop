using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Extensions;
using SynthShop.Domain.Settings;
using SynthShop.Infrastructure.Data.Interfaces;


namespace SynthShop.Core.Services.Impl
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly PagingSettings _pagingSettings;
        private readonly ILogger _logger;

        public CustomerService(ICustomerRepository customerRepository, ILogger logger, IOptions<PagingSettings> pagingSettings)
        {
            _customerRepository = customerRepository;
            _pagingSettings = pagingSettings.Value;
            _logger = logger.ForContext<CustomerService>();
        }

        public async Task CreateAsync(User user)
        {
            await _customerRepository.CreateAsync(user);
            _logger.Information("Customer created with ID {CustomerId}", user.Id);
        }

        public async Task<PagedList<User>> GetAllAsync(int? pageSize, int pageNumber = 1, string? searchTerm = null,
            string? sortBy = null, bool? IsAscending = true)
        {
            Expression<Func<User, bool>> filter = searchTerm is not null ?  x => x.FirstName.Contains(searchTerm) || x.LastName.Contains(searchTerm) || x.UserName.Contains(searchTerm) : null  ;
            return await _customerRepository.GetAllAsync(filter, sortBy, IsAscending ?? true, pageNumber, pageSize ?? _pagingSettings.PageSize);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<User?> UpdateAsync(Guid id, User updatedUser)
        {
            var existingUser = await _customerRepository.GetByIdAsync(id);
            if (existingUser == null)
            {
                _logger.Warning("Customer with ID {CustomerId} not found for update", id);
                return null;
            }

            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Email = updatedUser.Email;
            existingUser.Address = updatedUser.Address;
            existingUser.UpdateAt = DateTime.UtcNow;

            var updated = await _customerRepository.UpdateAsync(existingUser);
            _logger.Information("Customer with ID {CustomerId} updated", id);
            return updated;
        }

        public async Task<User?> DeleteAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                _logger.Warning("Customer with ID {CustomerId} not found for deletion", id);
                return null;
            }

            var deletedCustomer = await _customerRepository.DeleteAsync(customer);
            _logger.Information("Customer with ID {CustomerId} marked as deleted", id);
            return deletedCustomer;
        }
    }
}
