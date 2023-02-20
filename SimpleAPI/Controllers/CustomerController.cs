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
        public async Task<ActionResult> GetAllCustomersAsync()
        {

            var customers = await _customerService.GetAllCustomersAsync();
            if (customers == null)
            {
                return NotFound($"Customers not found");

            }
            else
            {
                _logger.LogInformation("Retrieving all customers");
                return Ok(customers.Select(r => _mapper.Map<CustomerDto>(r)).ToList());
            }
        }

        [HttpDelete("{customerId}")]
        public async Task<ActionResult> DeleteCustomerAsync(int customerId)
        {
            var response = await _customerService.DeleteCustomerAsync(customerId);
            _logger.LogInformation($"Deleted customer with ID {customerId}");

            if (response.Succeeded)
            {
                return Ok();
            }
            else
            {
                return ToResult(response);
            };
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