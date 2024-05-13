using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Core.Services.Impl
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger _logger;

        public CustomerService(ICustomerRepository customerRepository, ILogger logger)
        {
            _customerRepository = customerRepository;
            _logger = logger.ForContext<CustomerService>();
        }

        public async Task CreateAsync(User user)
        {
            if (user == null)
            {
                _logger.Warning("Attempted to create a null customer");
                return;
            }

            await _customerRepository.CreateAsync(user);
            _logger.Information("Customer created with ID {CustomerId}", user.Id);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                _logger.Warning("Customer with ID {CustomerId} not found", id);
                return null;
            }

            return customer;
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
