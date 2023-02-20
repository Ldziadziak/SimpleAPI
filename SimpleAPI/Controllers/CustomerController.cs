using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SimpleAPI.DTO;
using SimpleAPI.Models;
using SimpleAPI.Services;

namespace SimpleAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {

        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CustomerController(ILogger<CustomerController> logger, ICustomerService customer, IMapper mapper)
        {
            _logger = logger;
            _customerService = customer;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> AddCustomerAsync(CustomerDto dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            var response = await _customerService.AddCustomerAsync(customer);
            //if something went wrong return BadRequest(errs); add validations

            _logger.LogInformation($"Added customer with ID {customer.Id}");

            if (response.Succeeded)
            {
                return Ok();
            }
            else
            {
                return ToResult(response);
            };
        }


        [HttpGet]
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            _logger.LogInformation("Retrieving all customers");
            return await _customerService.GetAllCustomersAsync();
        }

        [HttpDelete("{customerId}")]
        public async Task DeleteCustomerAsync(int customerId)
        {
            await _customerService.DeleteCustomerAsync(customerId);
            _logger.LogInformation($"Deleted customer with ID {customerId}");
            await Task.CompletedTask;
        }

        private ActionResult ToResult(Microsoft.AspNetCore.Identity.IdentityResult serviceResponse)
        {

            if (serviceResponse.Errors.Any(e => e.Code.Equals(ICustomerService.NotFoundErrorCode)))
            {
                return NotFound(serviceResponse.Errors);
            }
            else if (serviceResponse.Errors.Any(e => e.Code.Equals(ICustomerService.DbErrorCode)))
            {
                return StatusCode(500, serviceResponse.Errors);
            }
            else
            {
                return BadRequest(serviceResponse.Errors);
            }
        }

    }
}