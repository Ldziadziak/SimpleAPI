using Microsoft.AspNetCore.Mvc;
using SimpleAPI.Models;
using SimpleAPI.Services;

namespace SimpleAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {

        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerService _customer;

        public CustomerController(ILogger<CustomerController> logger, ICustomerService customer)
        {
            _logger = logger;
            _customer = customer;
        }

        [HttpPost]
        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            //it should be new model CustomerInputModel withoud Id, and mapped to Customer
            await _customer.AddCustomerAsync(customer);
            _logger.LogInformation($"Added customer with ID {customer.Id}");
            return await Task.FromResult(customer);
        }

        [HttpGet]
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            _logger.LogInformation("Retrieving all customers");
            return await _customer.GetAllCustomersAsync();
        }

        [HttpDelete("{customerId}")]
        public async Task DeleteCustomerAsync(int customerId)
        {
            await _customer.DeleteCustomerAsync(customerId);
            _logger.LogInformation($"Deleted customer with ID {customerId}");
            await Task.CompletedTask;
        }

    }
}