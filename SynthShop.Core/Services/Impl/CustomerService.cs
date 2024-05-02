using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SynthShop.Infrastructure.Data.Interfaces;

namespace SynthShop.Core.Services.Impl
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task CreateAsync(User user)
        {
            await _customerRepository.CreateAsync(user);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<User?> UpdateAsync(Guid id, User user)
        {
            return await _customerRepository.UpdateAsync(id, user);
        }

        public async Task<User?> DeleteAsync(Guid id)
        {
            return await _customerRepository.DeleteAsync(id);
        }

    }
}
