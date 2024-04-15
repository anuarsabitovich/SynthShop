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
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductController(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddProductDTO addProductDTO)
        {
            var product = _mapper.Map<Product>(addProductDTO);
            await _productRepository.CreateAsync(product);
            return Ok(_mapper.Map<AddProductDTO>(product));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(_mapper.Map<List<ProductDTO>>(products));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductDTO>(product));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductDTO updateProductDTO)
        {
            var product = _mapper.Map<Product>(updateProductDTO);

            product = await _productRepository.UpdateAsync(id, product);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UpdateProductDTO>(product));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedProduct = await _productRepository.DeleteAsync(id);

            if (deletedProduct == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductDTO>(deletedProduct));
        }
    }



}
