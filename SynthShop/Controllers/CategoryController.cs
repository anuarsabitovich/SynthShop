using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Data.DTO;
using SynthShop.Data.Entities;
using SynthShop.Repositories;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCategoryDTO addCategoryDTO)
        {
            var categoryDomainModel = _mapper.Map<Category>(addCategoryDTO);
            await _categoryRepository.CreateAsync(categoryDomainModel);
            return Ok(_mapper.Map<AddCategoryDTO>(categoryDomainModel));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedCategory = await _categoryRepository.DeleteAsync(id);

            if (deletedCategory == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CategoryDTO>(deletedCategory));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return Ok(_mapper.Map<List<CategoryDTO>>(categories));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CategoryDTO>(category));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCategoryDTO updateCategoryDTO)
        {
            var categoryDomainModel = _mapper.Map<Category>(updateCategoryDTO);

            categoryDomainModel = await _categoryRepository.UpdateAsync(id, categoryDomainModel);

            if (categoryDomainModel == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CategoryDTO>(categoryDomainModel));
        }
    }
}
