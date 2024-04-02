using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Data.Entities;
using SynthShop.Repositories;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Order order)
        {
            await orderRepository.CreateAsync(order);
            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await orderRepository.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var order = await orderRepository.GetByIdAsync(id);

            if(order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, Order order)
        {
            var updateOrder = await orderRepository.UpdateAsync(id, order);
            if(order == null)
            {
                return NotFound();
            }
            return Ok(updateOrder);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id )
        {
            var order = await orderRepository.DeleteAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }


    }
}
