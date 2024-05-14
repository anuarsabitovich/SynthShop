using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Core.Services.Impl;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.Domain.Extensions;
using SynthShop.DTO;
using SynthShop.Queries;
using SynthShop.Validations;

using ILogger = Serilog.ILogger;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly ProductValidator _productValidator;
        private readonly ILogger _logger;

        public ProductController(IProductService productService, IMapper mapper, ProductValidator productValidator, ILogger logger)
        {
            _productService = productService;
            _mapper = mapper;
            _productValidator = productValidator;
            _logger = logger.ForContext<ProductController>();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddProductDTO addProductDTO)
        {
            var validationResult = _productValidator.Validate(addProductDTO);

            if (!validationResult.IsValid)
            {
                _logger.Warning("Validation failed for creating product. Errors: {@ValidationErrors}", validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            var product = _mapper.Map<Product>(addProductDTO);

            await _productService.CreateAsync(product);
            _logger.Information("Successfully created product {@Product}", product);
            return Ok(_mapper.Map<AddProductDTO>(product));
        }

        [HttpGet]
       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme) ]

        public async Task<IActionResult> GetAll( 
           //TODO validate inputs 
           
           [FromQuery] SearchQueryParameters searchQueryParameters
           )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var products = await _productService.GetAllAsync(searchQueryParameters.PageSize, searchQueryParameters.PageNumber,  searchQueryParameters.SearchTerm, searchQueryParameters.SortBy, searchQueryParameters.IsAscending ?? true);
            return Ok(_mapper.Map<PagedList<ProductDTO> >(products));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                _logger.Warning("Product not found with ID {ProductId}", id);
                return NotFound();
            }
            _logger.Information("Retrieved product {@Product}", product);
            return Ok(_mapper.Map<ProductDTO>(product));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] AddProductDTO updateProductDTO)
        {
            var validationResult = _productValidator.Validate(updateProductDTO);

            if (!validationResult.IsValid)
            {
                _logger.Warning("Validation failed for updating product. Errors: {@ValidationErrors}", validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }

            var product = _mapper.Map<Product>(updateProductDTO);

            product = await _productService.UpdateAsync(id, product);

            if (product == null)
            {
                _logger.Warning("Failed to update product with ID {ProductId}", id);
                return NotFound();
            }

            _logger.Information("Successfully updated product {@Product}", product);
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
            _logger.Information("Successfully deleted product with ID {ProductId}", id);
            return Ok(_mapper.Map<ProductDTO>(deletedProduct));
        }
    }



}
