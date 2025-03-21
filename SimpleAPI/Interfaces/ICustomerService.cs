using Microsoft.AspNetCore.Identity;
using SimpleAPI.Models;
using SimpleAPI.Services;

namespace SimpleAPI.Interfaces;
public interface ICustomerService
{
  const string DuplicateErrorCode = "Duplicate";
  const string DbErrorCode = "DbError";
  const string NotFoundErrorCode = "NotFound";
  const string InvalidDataErrorCode = "InvalidData";
  Task<IdentityResult> AddCustomerAsync(CustomerModel customer);
  Task<IEnumerable<CustomerModel?>> GetAllCustomersAsync();
  Task<Entities.Customer?> GetCustomerAsync(int Id);
  Task<IdentityResult> DeleteCustomerAsync(int Id);
  Task<bool> SaveChangesAsync();
  Task<IdentityResult> CustomerExistAsync(int customerId);
  Task<(IEnumerable<CustomerModel>, PaginationMetadata)> GetCustomersAsync(string? name, string? searchQuery, int pageNumber, int pageSize);
}
