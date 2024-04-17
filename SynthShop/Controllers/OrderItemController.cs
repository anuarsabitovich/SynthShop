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
    public class OrderItemController : ControllerBase
    {
        private readonly OrderItemService _orderItemService;
        private readonly IMapper _mapper;

        public OrderItemController(OrderItemService orderItemService, IMapper mapper)
        {
            _orderItemService = orderItemService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddOrderItemDTO addOrderItemDTO)
        {
            var orderItem = _mapper.Map<OrderItem>(addOrderItemDTO);
            await _orderItemService.CreateAsync(orderItem);
            return Ok(_mapper.Map<AddOrderItemDTO>(orderItem));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orderItems = await _orderItemService.GetAllAsync();
            return Ok(_mapper.Map<List<OrderItemDTO>>(orderItems));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var orderItem = await _orderItemService.GetByIdAsync(id);

            if (orderItem == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<OrderItemDTO>(orderItem));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateOrderItemDTO updateOrderItemDTO)
        {
            var orderItem = _mapper.Map<OrderItem>(updateOrderItemDTO);

            orderItem = await _orderItemService.UpdateAsync(id, orderItem);

            if (orderItem == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UpdateOrderItemDTO>(orderItem));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedOrderItem = await _orderItemService.DeleteAsync(id);

            if (deletedOrderItem == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<OrderItemDTO>(deletedOrderItem));
        }




    }
}
