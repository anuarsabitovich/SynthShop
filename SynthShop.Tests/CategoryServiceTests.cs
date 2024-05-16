using Microsoft.Extensions.Options;
using SynthShop.Core.Services.Impl;
using SynthShop.Domain.Settings;
using SynthShop.Infrastructure.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Serilog;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Extensions;
using System.Linq.Expressions;
using SynthShop.Tests.Extensions;

namespace SynthShop.Tests
{
    public class CategoryServiceTests
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger _logger;
        private readonly IOptions<PagingSettings> _pagingSettings;
        private readonly CategoryService _sut;

        public CategoryServiceTests()
        {
            _categoryRepository = Substitute.For<ICategoryRepository>();
            _logger = Substitute.For<ILogger>();
            _pagingSettings = Options.Create(new PagingSettings { PageSize = 10 });

            _sut = new CategoryService(_categoryRepository, _logger, _pagingSettings);
        }

        [Fact]
        public async Task CreateAsync_ShouldLogWarningAndThrow_WhenCategoryNameIsDuplicate()
        {
            var category = new Category { CategoryID = Guid.NewGuid(), Name = "Duplicate" };
            Expression<Func<Category, bool>> filter = x => x.Name.Contains(category.Name, StringComparison.OrdinalIgnoreCase);
            var existingCategories = new PagedList<Category>(new List<Category> { category }, 1, 1, 10);

            _categoryRepository.GetAllAsync(filter).Returns(Task.FromResult(existingCategories));
            _categoryRepository.GetAllAsync(Arg.Is<Expression<Func<Category, bool>>>(expr =>
                    ExpressionEqualityComparer.Instance.Equals(expr, filter)))
                .Returns(Task.FromResult(existingCategories));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(category));
            Assert.Equal("Category with name 'Duplicate' already exists.", exception.Message);
        }


    }
}
