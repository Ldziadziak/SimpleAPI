using SimpleAPI.Models;
using SimpleAPI.Services;

namespace SimpleAPI.Interfaces;

public interface ICustomerStore
{
    Task<CustomerModel> AddCustomerAsync(CustomerModel customer);
    Task<Entities.Customer> GetByIdAsync(int customerId);
    Task<IEnumerable<CustomerModel>> GetAllAsync();
    Task DeleteAsync(int customerId);
    Task<int> SaveChangesAsync();
    Task<(IEnumerable<CustomerModel>, PaginationMetadata)> GetAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
}
