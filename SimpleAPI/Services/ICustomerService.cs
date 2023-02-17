using SimpleAPI.Models;

namespace SimpleAPI.Services
{
    public interface ICustomerService
    {

        Task<Customer> AddCustomerAsync(Customer customer);
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task DeleteCustomerAsync(int Id);
    }
}
