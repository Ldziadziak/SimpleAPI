using SimpleAPI.Models;

namespace SimpleAPI.Data
{
    public interface ICustomerStore
    {
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<Customer> GetByIdAsync(int customerId);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task DeleteAsync(int customerId);
    }
}
