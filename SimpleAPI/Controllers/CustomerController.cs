using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SimpleAPI.DTO;
using SimpleAPI.Interfaces;
using SimpleAPI.Models;

namespace SimpleAPI.Controllers;

[ApiController]
[ApiVersion("1.1")]
[Route("api/customers")]
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

    [HttpPost("AddCustomer")]
    public async Task<ActionResult> AddCustomerAsync([FromBody] CustomerDto dto)
    {
        var customer = _mapper.Map<Customer>(dto);
        var response = await _customerService.AddCustomerAsync(customer);

        _logger.LogInformation($"Added customer with ID {customer.Id}");

        if (response.Succeeded)
        {
            return CreatedAtRoute("GetCustomer", new { customerId = customer.Id }, dto);
        }
        else
        {
            return ToResult(response);
        }
    }

    [HttpGet("GetCustomer/{customerId}", Name = "GetCustomer")]
    [Produces("application/json")]
    public async Task<ActionResult<Customer>> GetCustomersAsync(int customerId)
    {
        var customer = await _customerService.GetCustomerAsync(customerId);
        if (customer == null)
        {
            return NotFound($"Customer not found");
        }
        else
        {
            _logger.LogInformation("Retrieving customer");
            return Ok(customer);
        }
    }


    [HttpGet("GetCustomers")]
    [Produces("application/json")]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomersAsync()
    {

        var customers = await _customerService.GetAllCustomersAsync();
        if (customers == null)
        {
            return NotFound($"Customers not found");

        }
        else
        {
            _logger.LogInformation("Retrieving all customers");
            return Ok(customers);
        }
    }

    [HttpDelete("DeleteCustomer/{customerId}")]
    public async Task<ActionResult> DeleteCustomerAsync(int customerId)
    {
        var response = await _customerService.DeleteCustomerAsync(customerId);

        if (response.Succeeded)
        {
            _logger.LogInformation($"Deleted customer with ID {customerId}");
            return Ok();
        }
        else
        {
            _logger.LogInformation(response.ToString());
            return ToResult(response);
        }
    }

    [HttpPatch("{customerId}")]
    //[{ "operationType": 0, "path": "/name", "op": "replace", "value": "John" }]
    public async Task<ActionResult> PartiallyUpdateCustomer(int customerId, JsonPatchDocument<CustomerDto> patchDocument)
    {
        var customer = await _customerService.GetCustomerAsync(customerId);
        if (customer == null)
        {
            return NotFound();
        }

        var customerToPatch = _mapper.Map<CustomerDto>(customer);

        patchDocument.ApplyTo(customerToPatch, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(customerToPatch))
        {
            return BadRequest(ModelState);
        }

#pragma warning disable S1854 // Unnecessary assignment of a value
        customer = _mapper.Map(customerToPatch, customer);
#pragma warning restore S1854 // Unnecessary assignment of a value
        await _customerService.SaveChangesAsync();

        return NoContent();
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