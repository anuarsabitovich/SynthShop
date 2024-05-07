using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Core.Services.Impl;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.DTO;
using SynthShop.Validations;
using ILogger = Serilog.ILogger;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly OrderValidator _orderValidator;
        private readonly ILogger _logger;

        public OrderController(IOrderService orderService, IMapper mapper, OrderValidator orderValidator, ILogger logger)
        {
            _orderService = orderService;
            _mapper = mapper;
            _orderValidator = orderValidator;
            _logger = logger.ForContext<OrderController>();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDto)
        {
            var validationResult = _orderValidator.Validate(createOrderDto);

            if (!validationResult.IsValid)
            {
                _logger.Warning("Validation failed for creating order: {@ValidationErrors}", validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            var order = await _orderService.CreateOrder(createOrderDto.BasketId, createOrderDto.CustomerId);
            _logger.Information("Order created successfully with ID: {OrderId}", order.OrderID);
            return Ok(_mapper.Map<OrderDTO>(order));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] Guid id)
        {
            await _orderService.CancelOrder(id);
            _logger.Information("Order with ID {OrderId} deleted successfully", id);
            return NoContent();
        }

        [HttpPost]
        [Route("{id:Guid}")]
        public async Task<IActionResult> CompleteOrder([FromRoute] Guid id)
        {
            await _orderService.CompleteOrder(id);
            _logger.Information("Order with ID {OrderId} completed successfully", id);
            return Ok("Order completed");
        }


    }
}
