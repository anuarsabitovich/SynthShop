using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Constants;
using SynthShop.DTO;
using SynthShop.Extensions;
using SynthShop.Validations;
using ILogger = Serilog.ILogger;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Roles(RoleConstants.Admin, RoleConstants.User)]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly OrderValidator _orderValidator;
        private readonly ILogger _logger;
        private readonly IUserProvider _userProvider;

        public OrderController(IOrderService orderService, IMapper mapper, OrderValidator orderValidator, ILogger logger, IUserProvider userProvider)
        {
            _orderService = orderService;
            _mapper = mapper;
            _orderValidator = orderValidator;
            _userProvider = userProvider;
            _logger = logger.ForContext<OrderController>();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDto)
        {
            var validationResult = await _orderValidator.ValidateAsync(createOrderDto);

            if (!validationResult.IsValid)
            {
                _logger.Warning("Validation failed for creating order: {@ValidationErrors}", validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            var order = await _orderService.CreateOrder(createOrderDto.BasketId, _userProvider.GetCurrentUserId()!.Value);
            _logger.Information("Order created successfully with ID: {OrderId}", order.OrderID);
            return Ok(_mapper.Map<OrderDTO>(order));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] Guid id)
        {
            await _orderService.CancelOrder(id, _userProvider.GetCurrentUserId()!.Value);
            _logger.Information("Order with ID {OrderId} deleted successfully", id);
            return NoContent();
        }

        [HttpPost]
        [Route("complete/{id:Guid}")]
        public async Task<IActionResult> CompleteOrder([FromRoute] Guid id)
        {
            await _orderService.CompleteOrder(id, _userProvider.GetCurrentUserId()!.Value);
            _logger.Information("Order with ID {OrderId} completed successfully", id);
            return Ok("Order completed");
        }


    }
}
