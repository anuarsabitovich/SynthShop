using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Core.Services.Impl;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Domain.Intefaces;
using SynthShop.DTO;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly IMapper _mapper;

        public ProductController(ProductService productService , IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddProductDTO addProductDTO)
        {
            var product = _mapper.Map<Product>(addProductDTO);
            await _productService.CreateAsync(product);
            return Ok(_mapper.Map<AddProductDTO>(product));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(_mapper.Map<List<ProductDTO>>(products));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var product = await _productService.GetByIdAsync(id);

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

            product = await _productService.UpdateAsync(id, product);

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
            var deletedProduct = await _productService.DeleteAsync(id);

            if (deletedProduct == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<ProductDTO>(deletedProduct));
        }
    }



}
