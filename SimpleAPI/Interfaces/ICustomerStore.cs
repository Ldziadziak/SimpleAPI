using SimpleAPI.Models;
namespace SimpleAPI.Interfaces;

public interface ICustomerStore
{
    Task<Customer> AddCustomerAsync(Customer customer);
    Task<Entities.Customer> GetByIdAsync(int customerId);
    Task<IEnumerable<Customer>> GetAllAsync();
    Task DeleteAsync(int customerId);
    Task<int> SaveChangesAsync();
}
