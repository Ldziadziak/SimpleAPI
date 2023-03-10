using Microsoft.AspNetCore.Identity;
using SimpleAPI.Models;

namespace SimpleAPI.Interfaces;
public interface ICustomerService
{
    const string DuplicateErrorCode = "Duplicate";
    const string DbErrorCode = "DbError";
    const string NotFoundErrorCode = "NotFound";
    const string InvalidDataErrorCode = "InvalidData";
    Task<IdentityResult> AddCustomerAsync(Customer customer);
    Task<IEnumerable<Customer?>> GetAllCustomersAsync();
    Task<Entities.Customer?> GetCustomerAsync(int Id);
    Task<IdentityResult> DeleteCustomerAsync(int Id);
    Task<bool> SaveChangesAsync();
    Task<IdentityResult> CustomerExistAsync(int customerId);
}
