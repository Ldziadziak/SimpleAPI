using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SimpleAPI.DTO;
using SimpleAPI.Interfaces;
using SimpleAPI.Models;

namespace SimpleAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/customers")]
public class CustomerController : ControllerBase
{

    private readonly ILogger<CustomerController> _logger;
    private readonly ICustomerService _customerService;
    private readonly IMapper _mapper;
    private readonly IMailService _mailService;

    public CustomerController(ILogger<CustomerController> logger, ICustomerService customer, IMapper mapper, IMailService mailService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _customerService = customer ?? throw new ArgumentNullException(nameof(customer));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
    }

    [HttpPost("AddCustomer")]
    public async Task<ActionResult> AddCustomerAsync([FromBody] CustomerDto dto)
    {
        var customer = _mapper.Map<Customer>(dto);
        var response = await _customerService.AddCustomerAsync(customer);

        if (response.Succeeded)
        {
            _logger.LogInformation($"Added customer with ID {customer.Id}");
            _mailService.Send("Added customer", $"Added customer with ID {customer.Id}");
            return CreatedAtRoute("GetCustomer", new { customerId = customer.Id }, dto);
        }
        else
        {
            return ToResult(response);
        }
    }

    [HttpGet("GetCustomer/{customerId}", Name = "GetCustomer")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("DeleteCustomer/{customerId}")]
    public async Task<ActionResult> DeleteCustomerAsync(int customerId)
    {
        var response = await _customerService.DeleteCustomerAsync(customerId);

        if (response.Succeeded)
        {
            _logger.LogInformation($"Deleted customer with ID {customerId}");
            _mailService.Send("Deleted customer", $"Deleted customer with ID {customerId}");
            return Ok();
        }
        else
        {
            _logger.LogInformation(response.ToString());
            return ToResult(response);
        }
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPatch("Patch/Customer/{customerId}")]
    //[{ "operationType": 0, "path": "/name", "op": "replace", "value": "John" }]
    public async Task<ActionResult> UpdateCustomer(int customerId, JsonPatchDocument<CustomerDto> patchDocument)
    {
        var customerEntity = await _customerService.GetCustomerAsync(customerId);
        if (customerEntity == null)
        {
            return NotFound();
        }

        var customerToPatch = _mapper.Map<CustomerDto>(customerEntity);

        patchDocument.ApplyTo(customerToPatch, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!TryValidateModel(customerToPatch))
        {
            return BadRequest(ModelState);
        }

        _mapper.Map(customerToPatch, customerEntity);
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