using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Serilog;
using SynthShop.Core.Services.Impl;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Extensions;
using SynthShop.Domain.Settings;
using SynthShop.DTO;
using SynthShop.Infrastructure.Data.Interfaces;
using Xunit;

namespace SynthShop.Tests
{
    public class CustomerServiceTests
    {
        private readonly ICustomerService _sut;
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger _logger;

        public CustomerServiceTests()
        {
            _customerRepository = Substitute.For<ICustomerRepository>();
            _logger = Substitute.For<ILogger>();

            var pagingSettings = Options.Create(new PagingSettings { PageSize = 10 });
            _sut = new CustomerService(_customerRepository, _logger, pagingSettings);
        }
        
        [Fact]
        public async Task GetByIdAsync_ShouldReturnCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new User { Id = customerId, Email = "anuarkulmagambetov@gmail.com" };

            _customerRepository.GetByIdAsync(customerId).Returns(Task.FromResult(customer));

            // Act
            var result = await _sut.GetByIdAsync(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerId, result.Id);
            Assert.Equal("anuarkulmagambetov@gmail.com", result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            // Arrange
            _customerRepository.GetByIdAsync(Arg.Any<Guid>()).ReturnsNull();

            // Act
            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPagedList_WhenCalled()
        {
            // Arrange
            var pageSize = 10;
            var pageNumber = 1;
            var searchTerm = "John";
            var sortBy = "FirstName";
            var isAscending = true;

            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
                new User { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" }
            };
            var pagedList = new PagedList<User>(users, users.Count, pageNumber, pageSize);

            _customerRepository.GetAllAsync(Arg.Any<Expression<Func<User, bool>>>(), sortBy, isAscending, pageNumber, pageSize).Returns(Task.FromResult(pagedList));

            // Act
            var result = await _sut.GetAllAsync(pageSize, pageNumber, searchTerm, sortBy, isAscending);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal("john.doe@example.com", result.Items.First().Email);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var existingUser = new User
                { Id = customerId, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" };
            var updatedUser = new User { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" };
            _customerRepository.GetByIdAsync(customerId).Returns(Task.FromResult(existingUser));
            _customerRepository.UpdateAsync(existingUser).Returns(Task.FromResult(existingUser));
            
            // Act

            var result = await _sut.UpdateAsync(customerId, updatedUser);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal("Jane", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("jane.doe@example.com", result.Email);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var updatedUser = new User { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" };
            _customerRepository.GetByIdAsync(customerId).Returns(Task.FromResult<User>(null));
            
            // Act
            var result = await _sut.UpdateAsync(customerId, updatedUser);
            
            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnDeletedCustomer_WhenCustomerExists()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new User { Id = customerId, Email = "johndoe@example.com", IsDeleted = false };

            _customerRepository.GetByIdAsync(customerId).Returns(Task.FromResult(customer));

            _customerRepository.DeleteAsync(Arg.Do<User>(x => x.IsDeleted = true))
                .Returns(Task.FromResult(customer));

            // Act
            var result = await _sut.DeleteAsync(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsDeleted);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnNull_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _customerRepository.GetByIdAsync(customerId).Returns(Task.FromResult<User>(null));

            // Act
            var result = await _sut.DeleteAsync(customerId);

            // Assert
            Assert.Null(result);
        }

     
    }
}
