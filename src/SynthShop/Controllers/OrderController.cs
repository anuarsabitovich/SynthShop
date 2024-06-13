using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Constants;
using SynthShop.Domain.Exceptions;
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

            var orderResult = await _orderService.CreateOrder(createOrderDto.BasketId, _userProvider.GetCurrentUserId()!.Value);

            return orderResult.Match<IActionResult>(
                result => Ok(_mapper.Map<OrderDTO>(result)),
                exception =>
                {
                    return exception switch
                    {
                        OrderFailedException => BadRequest(exception.Message),
                        DbUpdateConcurrencyException => Conflict(new { message = "Please, try to create order later"}),
                        _ => StatusCode(500, "An unexpected error occurred.")
                    };
                });
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] Guid id)
        {
            var cancelOrderResult =  await _orderService.CancelOrder(id, _userProvider.GetCurrentUserId()!.Value);
            return cancelOrderResult.Match<IActionResult>(
                result => Ok(_mapper.Map<OrderDTO>(result)),

                exception =>
                {
                    return exception switch
                    {
                        InvalidOperationException => BadRequest(new { message = exception.Message }),
                        _ => StatusCode(500, "An unexpected error occurred.")
                    };
                });
        }

        [HttpPost]
        [Route("complete/{id:Guid}")]
        public async Task<IActionResult> CompleteOrder([FromRoute] Guid id)
        {
            var completedOrder =  await _orderService.CompleteOrder(id, _userProvider.GetCurrentUserId()!.Value);
            return completedOrder.Match<IActionResult>(
                result => Ok(_mapper.Map<OrderDTO>(result)),
                exception =>
                {
                    return exception switch
                    {
                        InvalidOperationException => BadRequest(new { message = exception.Message }),
                        _ => StatusCode(500, "An unexpected error occured")
                    };
                }
            );
        }

        [HttpGet]
        [Route("customer-orders")]
        public async Task<IActionResult> GetOrdersByCustomerId()
        {
            var customerId = _userProvider.GetCurrentUserId();

            if (customerId == null)
            {
                return Unauthorized("Customer ID not found.");
            }

            var ordersResult = await _orderService.GetOrdersByCustomerId(customerId.Value);

            return ordersResult.Match<IActionResult>(
                result => Ok(_mapper.Map<List<OrderDTO>>(result)),
                exception =>
                {
                    return exception switch
                    {
                        _ => StatusCode(500, "An unexpected error occurred.")
                    };
                });
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetOrder([FromRoute] Guid id)
        {
            var orderResult = await _orderService.GetOrderByIdAsync(id);

            return orderResult.Match<IActionResult>(
                result => Ok(_mapper.Map<OrderDTO>(result)),
                exception =>
                {
                    return exception switch
                    {
                        InvalidOperationException => BadRequest(new { message = exception.Message }),
                        _ => StatusCode(500, "An unexpected error occurred.")
                    };
                });
        }

    }
}
