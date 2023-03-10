using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SimpleAPI.Interfaces;
using SimpleAPI.Models;

namespace SimpleAPI.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerStore _customerStore;
    private readonly ILogger<CustomerService> _logger;
    public CustomerService(ICustomerStore customerStore, ILogger<CustomerService> logger)
    {
        _customerStore = customerStore ?? throw new ArgumentNullException(nameof(customerStore));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IdentityResult> AddCustomerAsync(Customer customer)
    {
        //use valid
        try
        {
            await _customerStore.AddCustomerAsync(customer);
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError() { Code = ICustomerService.DbErrorCode, Description = ex.Message });
        }
        return IdentityResult.Success;
    }

    public async Task<IEnumerable<Customer?>> GetAllCustomersAsync()
    {
        var customers = await _customerStore.GetAllAsync();
        if (customers == null)
        {
            return null!;
        }

        return customers;
    }

    public async Task<Entities.Customer?> GetCustomerAsync(int Id)
    {
        var customerExistAsync = await CustomerExistAsync(Id);

        if (customerExistAsync == IdentityResult.Success)
        {
            return await _customerStore.GetByIdAsync(Id);
        }

        return null!;
    }

    public async Task<IdentityResult> CustomerExistAsync(int Id)
    {
        try
        {
            await _customerStore.GetByIdAsync(Id);
        }
        catch (EntityNotFoundException ex)
        {
            _logger.LogCritical($"Exception while geting customer {Id}", ex);
            return IdentityResult.Failed(new IdentityError() { Code = ICustomerService.NotFoundErrorCode, Description = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Exception while geting customer {Id}", ex);
            return IdentityResult.Failed(new IdentityError() { Code = ICustomerService.DbErrorCode, Description = ex.Message });
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteCustomerAsync(int Id)
    {
        var customerExistAsync = await CustomerExistAsync(Id);

        if (customerExistAsync == IdentityResult.Success)
        {
            try
            {
                await _customerStore.DeleteAsync(Id);
                await _customerStore.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while deleting customer {Id}", ex);
                return IdentityResult.Failed(new IdentityError() { Code = ICustomerService.DbErrorCode, Description = ex.Message });
            }
        }

        return customerExistAsync;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return (await _customerStore.SaveChangesAsync() >= 0);
    }
}
