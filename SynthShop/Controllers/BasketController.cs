using AutoMapper;
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
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IMapper _mapper;
        private readonly BasketItemValidator _basketItemValidator;

        public BasketController(IBasketService basketService, IMapper mapper, BasketItemValidator basketItemValidator)
        {
            _basketService = basketService;
            _mapper = mapper;
            _basketItemValidator = basketItemValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            await _basketService.CreateBasketAsync();
            return Ok("Basket Created");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid basketId)
        {
            await _basketService.DeleteBasketAsync(basketId);
            return NoContent();
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetBaskedById([FromRoute] Guid id)
        {
            var basket = await _basketService.GetBasketByIdAsync(id);
            if (basket == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<BasketDTO>(basket));
        }

        [HttpPost]
        [Route("{id:Guid}")]
        public async Task<IActionResult> AddItemToBasket( [FromRoute] Guid id,[FromBody] AddBasketItemDTO addBasketItemDto)
        {
            var validationResult = await _basketItemValidator.ValidateAsync(addBasketItemDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
            await _basketService.AddItemToBasketAsync(id, addBasketItemDto.ProductId, addBasketItemDto.Quantity);
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
            var validationResult = await _basketItemValidator.ValidateAsync(updateBaskItemDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            await _basketService.UpdateItemInBasket(id, updateBaskItemDto.ProductId, updateBaskItemDto.Quantity);
            return Ok("Item in the basket was updated");
        }
        

    }
}
