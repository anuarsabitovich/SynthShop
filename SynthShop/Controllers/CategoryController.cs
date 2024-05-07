﻿using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog.Core;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.DTO;
using SynthShop.Validations;
using ILogger = Serilog.ILogger;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly CategoryValidator _categoryValidator;
        private readonly ILogger _logger;

        public CategoryController(ICategoryService categoryService, IMapper mapper, CategoryValidator categoryValidator, ILogger logger)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _categoryValidator = categoryValidator;
            _logger = logger.ForContext<CategoryController>(); // Contextual logging
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCategoryDTO addCategoryDTO)
        {
            
            var validationResult = _categoryValidator.Validate(addCategoryDTO);
            if (validationResult.IsValid == false)
            {
                _logger.Warning("Validation failed for creating category. Errors: {@ValidationErrors}", validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            var categoryDomainModel = _mapper.Map<Category>(addCategoryDTO);
            
            await _categoryService.CreateAsync(categoryDomainModel);
            _logger.Information("Successfully created a new category with ID {@CategoryId}", addCategoryDTO);
            return Ok(_mapper.Map<AddCategoryDTO>(categoryDomainModel));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedCategory = await _categoryService.DeleteAsync(id);

            if (deletedCategory == null)
            {
                _logger.Warning("Failed to find category with ID {CategoryId} to delete", id);
                return NotFound();
            }
            _logger.Information("Successfully deleted category with ID {CategoryId}", id);
            return Ok(_mapper.Map<CategoryDTO>(deletedCategory));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            
            var categories = await _categoryService.GetAllAsync();
            return Ok(_mapper.Map<List<CategoryDTO>>(categories));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                _logger.Warning("Category with ID: {CategoryId} not found", id);
                return NotFound();
            }
            _logger.Information("Category with ID: {CategoryId} retrieved successfully", id);
            return Ok(_mapper.Map<CategoryDTO>(category));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] AddCategoryDTO updateCategoryDTO)
        {
            var validationResult = _categoryValidator.Validate(updateCategoryDTO);
            if (validationResult.IsValid == false)
            {
                _logger.Warning("Validation failed for updating category with ID: {CategoryId}. Errors: {@ValidationErrors}", id, validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            var categoryDomainModel = _mapper.Map<Category>(updateCategoryDTO);

          

            categoryDomainModel = await _categoryService.UpdateAsync(id, categoryDomainModel);

            if (categoryDomainModel == null)
            {
                _logger.Warning("Failed to update category with ID: {CategoryId}", id);
                return NotFound();
            }
            _logger.Information("Category with ID: {CategoryId} updated successfully", id);
            return Ok(_mapper.Map<CategoryDTO>(categoryDomainModel));
        }
    }
}
