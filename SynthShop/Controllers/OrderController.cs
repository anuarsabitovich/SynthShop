using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Core.Services.Impl;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Entities;
using SynthShop.DTO;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO createOrderDto)
        {
           var order =  await _orderService.CreateOrder(createOrderDto.BasketId, createOrderDto.CustomerId);
           return Ok(_mapper.Map<OrderDTO>(order));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteOrder([FromRoute] Guid id)
        {
            await _orderService.CancelOrder(id);
            return NoContent();
        }

        [HttpPost]
        [Route("{id:Guid}")]
        public async Task<IActionResult> CompleteOrder([FromRoute] Guid id)
        {
            await _orderService.CompleteOrder(id);
            return Ok("Order completed");
        }
       

    }
}
