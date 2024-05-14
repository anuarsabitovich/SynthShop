using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SynthShop.Core.Services.Impl;
using SynthShop.Domain.Entities;
using SynthShop.DTO;
using SynthShop.Core.Services.Interfaces;
using SynthShop.Domain.Extensions;
using SynthShop.Validations;
using ILogger = Serilog.ILogger;
using SynthShop.Queries;

namespace SynthShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly CustomerValidator _customerValidator;
        private readonly QueryParametersValidator _queryParametersValidator;
        private readonly ILogger _logger;

        public UserController(ICustomerService customerService, IMapper mapper, CustomerValidator customerValidator, ILogger logger, QueryParametersValidator queryParametersValidator)
        {
            _customerService = customerService;
            _mapper = mapper;
            _customerValidator = customerValidator;
            _logger = logger.ForContext<UserController>(); // Use ForContext to specify the controller
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCustomerDTO addCustomerDTO)
        {
            var validationResult = _customerValidator.Validate(addCustomerDTO);

            if (!validationResult.IsValid)
            {
                _logger.Warning("Failed validation for creating user {@Errors}", validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }

            var customer = _mapper.Map<User>(addCustomerDTO);

            await _customerService.CreateAsync(customer);
            _logger.Information("Created a new user {@User}", customer);
            return Ok(_mapper.Map<AddCustomerDTO>(customer));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SearchQueryParameters searchQueryParameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var customers = await _customerService.GetAllAsync(searchQueryParameters.PageSize, searchQueryParameters.PageNumber, searchQueryParameters.SearchTerm, searchQueryParameters.SortBy, searchQueryParameters.IsAscending ?? true );
            return Ok(_mapper.Map<PagedList<CustomerDTO>>(customers));
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            if (customer == null)
            {
                _logger.Warning("User not found with ID {UserId}", id);
                return NotFound();
            }
            _logger.Information("Retrieved user details for {UserId}", id);
            return Ok(_mapper.Map<CustomerDTO>(customer));

        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] AddCustomerDTO updateCustomerDTO)
        {
            var validationResult = _customerValidator.Validate(updateCustomerDTO);

            if (!validationResult.IsValid)
            {
                _logger.Warning("Failed validation for updating user {@Errors}", validationResult.Errors);
                return BadRequest(validationResult.Errors);
            }
            
            var customer = _mapper.Map<User>(updateCustomerDTO);

            customer = await _customerService.UpdateAsync(id, customer);
            
            if (customer == null)
            {
                _logger.Warning("User not found or could not be updated with ID {UserId}", id);
                return NotFound();
            }

            _logger.Information("Updated user {UserId}", id);
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
            _logger.Information("Deleted user with ID {UserId}", id);
            return Ok(_mapper.Map<CustomerDTO>(deletedCustomer));
        }
    }
}
