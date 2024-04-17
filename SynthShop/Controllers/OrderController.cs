﻿using AutoMapper;
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
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly IMapper _mapper;

        public OrderController(OrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddOrderDTO addOrderDTO)
        {
            var order = _mapper.Map<Order>(addOrderDTO);
            await _orderService.CreateAsync(order);
            return Ok(_mapper.Map<AddOrderDTO>(order));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(_mapper.Map<List<OrderDTO>>(orders));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<OrderDTO>(order));
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateOrderDTO updateOrderDTO)
        {
            var order = _mapper.Map<Order>(updateOrderDTO);

            order = await _orderService.UpdateAsync(id, order);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<OrderDTO>(order));
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedOrder = await _orderService.DeleteAsync(id);

            if (deletedOrder == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<OrderDTO>(deletedOrder));
        }


    }
}
