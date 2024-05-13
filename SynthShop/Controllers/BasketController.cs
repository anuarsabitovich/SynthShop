using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.DTO;
using SynthShop.Validations;
using Serilog;
using ILogger = Serilog.ILogger;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IMapper _mapper;
        private readonly BasketItemValidator _basketItemValidator;
        private readonly ILogger _logger;

        public BasketController(IBasketService basketService, IMapper mapper, BasketItemValidator basketItemValidator, ILogger logger)
        {
            _basketService = basketService;
            _mapper = mapper;
            _basketItemValidator = basketItemValidator;
            _logger = logger.ForContext<BasketController>();
        }
        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var basketId = await _basketService.CreateBasketAsync();
            return Ok($"Created basket with id {basketId}" );
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid basketId)
        {
            _logger.Information("Deleting basket with ID {BasketId}", basketId);
            await _basketService.DeleteBasketAsync(basketId);
            _logger.Information("Basket with ID {BasketId} deleted", basketId);
            return NoContent();
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetBaskedById([FromRoute] Guid id)
        {
            _logger.Information("Fetching basket by ID {BasketId}", id);
            var basket = await _basketService.GetBasketByIdAsync(id);
            if (basket == null)
            {
                _logger.Warning("Basket not found with ID {BasketId}", id);
                return NotFound();
            }
            _logger.Information("Basket retrieved for ID {BasketId}", id);
            return Ok(_mapper.Map<BasketDTO>(basket));
        }

        [HttpPost]
        [Route("{id:Guid}")]
        public async Task<IActionResult> AddItemToBasket( [FromRoute] Guid id,[FromBody] AddBasketItemDTO addBasketItemDto)
        {
            _logger.Information("Adding item to basket {BasketId}", id);
            var validationResult = await _basketItemValidator.ValidateAsync(addBasketItemDto);
            if (!validationResult.IsValid)
            {
                _logger.Warning("Validation failed for adding item to basket {BasketId}: {Errors}", id, validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            
            await _basketService.AddItemToBasketAsync(id, addBasketItemDto.ProductId, addBasketItemDto.Quantity);
            _logger.Information("Item added to basket {BasketId}", id);
            return Ok("Item added to basket");
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteItemFromBasketAsync([FromRoute] Guid id, [FromBody] Guid basketId)
        {
           
            await _basketService.DeleteItemFromBasketAsync(id, basketId);
            
            return NoContent();
        }
        
        
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateItemInBasket([FromRoute] Guid id, [FromBody] AddBasketItemDTO updateBaskItemDto)
        {
            _logger.Information("Updating item in basket {BasketId}", id);
            var validationResult = await _basketItemValidator.ValidateAsync(updateBaskItemDto);
            if (!validationResult.IsValid)
            {
                _logger.Warning("Validation failed for updating item in basket {BasketId}: {Errors}", id, validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            await _basketService.UpdateItemInBasket(id, updateBaskItemDto.ProductId, updateBaskItemDto.Quantity);
            _logger.Information("Item in basket {BasketId} was updated", id);
            return Ok("Item in the basket was updated");
        }
    }
}
