using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Data.DTO;
using SynthShop.Data.Entities;
using SynthShop.Repositories;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerController(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCustomerDTO addCustomerDTO)
        {
            var customer = _mapper.Map<Customer>(addCustomerDTO);
            await _customerRepository.CreateAsync(customer);
            return Ok(_mapper.Map<AddCustomerDTO>(customer));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerRepository.GetAllAsync();
            return Ok(_mapper.Map<List<CustomerDTO>>(customers));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CustomerDTO>(customer));

        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCustomerDTO updateCustomerDTO)
        {
            var customer = _mapper.Map<Customer>(updateCustomerDTO);

            customer = await _customerRepository.UpdateAsync(id, customer);
            
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UpdateCustomerDTO>(customer));
        }


        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var deletedCustomer = await _customerRepository.DeleteAsync(id);

            if (deletedCustomer == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CustomerDTO>(deletedCustomer));
        }
    }
}
