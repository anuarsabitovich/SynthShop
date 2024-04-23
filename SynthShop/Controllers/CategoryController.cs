using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.DTO;
using SynthShop.Validations;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly CategoryValidator _categoryValidator;

        public CategoryController(ICategoryService categoryService, IMapper mapper, CategoryValidator categoryValidator)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _categoryValidator = categoryValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCategoryDTO addCategoryDTO)
        {
            var validationResult = _categoryValidator.Validate(addCategoryDTO);
            if (validationResult.IsValid == false)
            {
                return BadRequest(validationResult.Errors);
            }
            var categoryDomainModel = _mapper.Map<Category>(addCategoryDTO);
           
            await _categoryService.CreateAsync(categoryDomainModel);
            return Ok(_mapper.Map<AddCategoryDTO>(categoryDomainModel));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedCategory = await _categoryService.DeleteAsync(id);

            if (deletedCategory == null)
            {
                return NotFound();
            }

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
                return NotFound();
            }

            return Ok(_mapper.Map<CategoryDTO>(category));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] AddCategoryDTO updateCategoryDTO)
        {
            var validationResult = _categoryValidator.Validate(updateCategoryDTO);
            if (validationResult.IsValid == false)
            {
                return BadRequest(validationResult.Errors);
            }
            var categoryDomainModel = _mapper.Map<Category>(updateCategoryDTO);

          

            categoryDomainModel = await _categoryService.UpdateAsync(id, categoryDomainModel);

            if (categoryDomainModel == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CategoryDTO>(categoryDomainModel));
        }
    }
}
