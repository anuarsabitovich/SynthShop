using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Core.Services.Impl;
using SynthShop.Domain.Entities;
using SynthShop.Infrastructure.Domain.Intefaces;
using SynthShop.DTO;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Validations;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly CustomerValidator _customerValidator;

        public CustomerController(ICustomerService customerService, IMapper mapper, CustomerValidator customerValidator)
        {
            _customerService = customerService;
            _mapper = mapper;
            _customerValidator = customerValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCustomerDTO addCustomerDTO)
        {
            var customer = _mapper.Map<Customer>(addCustomerDTO);

            var validationResult = _customerValidator.Validate(customer);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors); 
            }

            await _customerService.CreateAsync(customer);
            return Ok(_mapper.Map<AddCustomerDTO>(customer));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(_mapper.Map<List<CustomerDTO>>(customers));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);

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

            var validationResult = _customerValidator.Validate(customer);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            customer = await _customerService.UpdateAsync(id, customer);
            
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
            var deletedCustomer = await _customerService.DeleteAsync(id);

            if (deletedCustomer == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CustomerDTO>(deletedCustomer));
        }
    }
}
