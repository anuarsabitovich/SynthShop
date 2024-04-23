﻿using SynthShop.Core.Services.Interfaces;
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

        public async Task CreateAsync(Customer customer)
        {
            await _customerRepository.CreateAsync(customer);
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _customerRepository.GetAllAsync();
        }

        public async Task<Customer?> GetByIdAsync(Guid id)
        {
            return await _customerRepository.GetByIdAsync(id);
        }

        public async Task<Customer?> UpdateAsync(Guid id, Customer customer)
        {
            return await _customerRepository.UpdateAsync(id, customer);
        }

        public async Task<Customer?> DeleteAsync(Guid id)
        {
            return await _customerRepository.DeleteAsync(id);
        }

    }
}
