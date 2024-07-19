using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Core.Services.Interfaces;
using SynthShop.DTO;
using SynthShop.Validations;
using ILogger = Serilog.ILogger;

namespace SynthShop.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;
    private readonly IMapper _mapper;
    private readonly AddBasketItemValidator _addBasketItemValidator;
    private readonly ILogger _logger;
    private readonly UpdateBasketItemValidator _updateBasketItemValidator;

    public BasketController(IBasketService basketService, IMapper mapper, AddBasketItemValidator addBasketItemValidator,
        ILogger logger, UpdateBasketItemValidator updateBasketItemValidator)
    {
        _basketService = basketService;
        _mapper = mapper;
        _addBasketItemValidator = addBasketItemValidator;
        _updateBasketItemValidator = updateBasketItemValidator;
        _logger = logger.ForContext<BasketController>();
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var basketId = await _basketService.CreateBasketAsync();
        return Ok(basketId);
    }

    [HttpDelete("basket/{basketId:Guid}")]
    public async Task<IActionResult> DeleteBasket([FromRoute] Guid basketId)
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

    [HttpPost("{id:Guid}/items")]
    public async Task<IActionResult> AddItemToBasket([FromRoute] Guid id, [FromBody] AddBasketItemDTO addBasketItemDto)
    {
        _logger.Information("Adding item to basket {BasketId}", id);
        var validationResult = await _addBasketItemValidator.ValidateAsync(addBasketItemDto);
        if (!validationResult.IsValid)
        {
            _logger.Error("Validation failed for adding item to basket {BasketId}: {@Errors}", id,
                validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }

        await _basketService.AddItemToBasketAsync(id, addBasketItemDto.ProductId, addBasketItemDto.Quantity);
        _logger.Information("Item added to basket {BasketId}", id);
        return Ok("Item added to basket");
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> DeleteItemFromBasketAsync([FromRoute] Guid id)
    {
        await _basketService.DeleteItemFromBasketAsync(id);

        return NoContent();
    }

    [HttpDelete("items/{itemId:Guid}/remove")]
    public async Task<IActionResult> RemoveItemFromBasketByOneAsync([FromRoute] Guid itemId)
    {
        await _basketService.RemoveBasketItemByOne(itemId);
        return Ok();
    }


    [HttpPut("{id:Guid}/items/{itemId:Guid}")]
    public async Task<IActionResult> UpdateItemInBasket([FromRoute] Guid id,
        [FromBody] UpdateBaskItemDTO updateBaskItemDto)
    {
        _logger.Information("Updating item in basket {BasketId}", id);
        var validationResult = await _updateBasketItemValidator.ValidateAsync(updateBaskItemDto);
        if (!validationResult.IsValid)
        {
            _logger.Warning("Validation failed for updating item in basket {BasketId}: {Errors}", id,
                validationResult.Errors);
            return BadRequest(validationResult.Errors);
        }

        await _basketService.UpdateItemInBasket(id, updateBaskItemDto.BasketItemId, updateBaskItemDto.Quantity);
        _logger.Information("Item in basket {BasketId} was updated", id);
        return Ok("Item in the basket was updated");
    }

    [HttpPut("{id:Guid}/customer")]
    public async Task<IActionResult> UpdateBasketCustomer([FromRoute] Guid id, [FromBody] CustomerIdDto customerIdDto)
    {
        _logger.Information("Updating customer ID for basket {BasketId}", id);
        var basket = await _basketService.UpdateBasketAsync(id, customerIdDto.CustomerId);
        if (basket == null)
        {
            _logger.Warning("Basket with ID {BasketId} not found", id);
            return NotFound();
        }
        _logger.Information("Customer ID for basket {BasketId} was updated to {CustomerId}", id, customerIdDto.CustomerId);
        return Ok("Customer ID was updated");
    }

    [HttpGet("last-basket/{customerId:Guid}")]
    public async Task<IActionResult> GetLastBasketByCustomerId([FromRoute] Guid customerId)
    {
        _logger.Information("Fetching last basket by customer ID {CustomerId}", customerId);
        var basket = await _basketService.GetLastBasketByCustomerIdAsync(customerId);
        if (basket == null)
        {
            _logger.Warning("No baskets found for customer ID {CustomerId}", customerId);
            return NotFound();
        }

        _logger.Information("Last basket retrieved for customer ID {CustomerId} with id {BasketId}", customerId, basket.BasketId);
        return Ok(_mapper.Map<BasketDTO>(basket));
    }

}