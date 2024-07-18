using Microsoft.Extensions.Options;
using SynthShop.Core.Services.Impl;
using SynthShop.Domain.Settings;
using SynthShop.Infrastructure.Data.Interfaces;
using NSubstitute;
using Serilog;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Extensions;
using System.Linq.Expressions;
using SynthShop.Tests.Extensions;
using SynthShop.Domain.Exceptions;

namespace SynthShop.Tests;

public class CategoryServiceTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger _logger;
    private readonly IOptions<PagingSettings> _pagingSettings;
    private readonly CategoryService _sut;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryServiceTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _logger = Substitute.For<ILogger>();
        _pagingSettings = Options.Create(new PagingSettings { PageSize = 10 });
        _unitOfWork = Substitute.For<IUnitOfWork>();

        _sut = new CategoryService(_categoryRepository, _logger, _pagingSettings, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateCategory_WhenCategoryNameIsUnique()
    {
        // Arrange
        var category = new Category { CategoryID = Guid.NewGuid(), Name = "Unique" };
        var existingCategories = new PagedList<Category>(new List<Category>(), 0, 1, 10);

        _categoryRepository.GetAllAsync(Arg.Any<Expression<Func<Category, bool>>>())
            .Returns(Task.FromResult(existingCategories));

        // Act
        await _sut.CreateAsync(category);

        // Assert
        await _categoryRepository.Received(1).CreateAsync(category);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedListOfCategories()
    {
        // Arrange
        var categories = new PagedList<Category>(new List<Category> { new() { Name = "Category1" } }, 1, 1, 10);
        _categoryRepository
            .GetAllAsync(Arg.Any<Expression<Func<Category, bool>>>(), Arg.Any<string>(), Arg.Any<bool>(),
                Arg.Any<int>(), Arg.Any<int>()).Returns(Task.FromResult(categories));

        // Act
        var result = await _sut.GetAllAsync(10, 1, "Category", "Name", true);

        // Assert
        Assert.Equal(categories, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategory_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { CategoryID = categoryId };
        _categoryRepository.GetByIdAsync(categoryId).Returns(Task.FromResult(category));

        // Act
        var result = await _sut.GetByIdAsync(categoryId);

        // Assert
        Assert.Equal(category, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _categoryRepository.GetByIdAsync(categoryId).Returns(Task.FromResult<Category>(null));

        // Act
        var result = await _sut.GetByIdAsync(categoryId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var updatedCategory = new Category { CategoryID = categoryId, Name = "Updated" };
        _categoryRepository.GetByIdAsync(categoryId).Returns(Task.FromResult<Category>(null));

        // Act
        var result = await _sut.UpdateAsync(categoryId, updatedCategory);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public async Task UpdateAsync_ShouldUpdateCategory_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var existingCategory = new Category { CategoryID = categoryId, Name = "Existing" };
        var updatedCategory = new Category { Name = "Updated", Description = "Updated Description" };
        _categoryRepository.GetByIdAsync(categoryId).Returns(Task.FromResult(existingCategory));
        _categoryRepository.UpdateAsync(Arg.Any<Category>()).Returns(Task.FromResult(updatedCategory));

        // Act
        var result = await _sut.UpdateAsync(categoryId, updatedCategory);

        // Assert
        Assert.Equal(updatedCategory.Name, existingCategory.Name);
        Assert.Equal(updatedCategory.Description, existingCategory.Description);
        await _categoryRepository.Received(1).UpdateAsync(existingCategory);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteAsync_WarningAndReturnNull_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _categoryRepository.GetByIdAsync(categoryId).Returns(Task.FromResult<Category>(null));

        // Act
        var result = await _sut.DeleteAsync(categoryId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteCategory_WhenCategoryExists()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { CategoryID = categoryId };
        _categoryRepository.GetByIdAsync(categoryId).Returns(Task.FromResult(category));
        _categoryRepository.DeleteAsync(category).Returns(Task.FromResult(category));

        // Act
        var result = await _sut.DeleteAsync(categoryId);

        // Assert
        Assert.Equal(category, result);
        await _categoryRepository.Received(1).DeleteAsync(category);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }
}