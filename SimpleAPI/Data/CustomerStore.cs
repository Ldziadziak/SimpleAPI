using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Controllers;
using SimpleAPI.DbContexts;
using SimpleAPI.Interfaces;
using SimpleAPI.Models;
using SimpleAPI.Services;

namespace SimpleAPI.Data;

public class DbCustomerStore(ILogger<CustomerController> logger, CustomerContext context, IMapper mapper) : ICustomerStore
{
  private readonly ILogger<CustomerController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  private readonly CustomerContext _context = context ?? throw new ArgumentNullException(nameof(context));
  private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

  public async Task<CustomerModel> AddCustomerAsync(CustomerModel customer)
  {
    var entity = _mapper.Map<Entities.Customer>(customer);
    _context.Add(entity);
    await _context.SaveChangesAsync();

    _mapper.Map<Entities.Customer, CustomerModel>(entity, customer);

    return await Task.FromResult(customer);
  }

  public async Task<Entities.Customer> GetByIdAsync(int customerId)
  {
    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);

    if (customer is null)
    {
      _logger.LogWarning("Failed to get customer with ID {CustomerId}", customerId);
      throw new EntityNotFoundException(customerId, $"Failed to get customer {customerId} from DB");
    }

    return customer;
  }

  public async Task<IEnumerable<CustomerModel>> GetAllAsync()
  {
    var customers = await _context.Customers.ToListAsync();
    return customers.Select(c => _mapper.Map<CustomerModel>(c));
  }

  public async Task<(IEnumerable<CustomerModel>, PaginationMetadata)> GetAsync(string? name, string? searchQuery, int pageNumber, int pageSize)
  {
    // collection to start from
    var collection = _context.Customers as IQueryable<Entities.Customer>;

    if (!string.IsNullOrWhiteSpace(name))
    {
      name = name.Trim();
      _ = collection.Where(c => c.Name.StartsWith(name));
    }

    if (!string.IsNullOrWhiteSpace(searchQuery))
    {
      searchQuery = searchQuery.Trim();
      collection = collection.Where(a =>
          (a.Name != null && a.Name.Contains(searchQuery)) ||
          (a.Surname != null && a.Surname.Contains(searchQuery)));
    }

    var totalItemCount = await collection.CountAsync();

    var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

    var collectionToReturn = await collection.OrderBy(c => c.Name)
        .Skip(pageSize * (pageNumber - 1))
        .Take(pageSize)
        .ToListAsync();

    return (collectionToReturn.Select(c => _mapper.Map<CustomerModel>(c)), paginationMetadata);
  }

  public async Task DeleteAsync(int customerId)
  {
    var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);

    if (customer is null)
    {
      _logger.LogWarning("Attempted to delete a non-existent customer with ID {CustomerId}", customerId);
      throw new EntityNotFoundException(customerId, $"Customer with ID {customerId} not found.");
    }

    _context.Customers.Remove(customer);
    await _context.SaveChangesAsync();

    _logger.LogInformation("Deleted customer with ID {CustomerId}", customerId);
  }

  public async Task<int> SaveChangesAsync()
  {
    return await _context.SaveChangesAsync();
  }
}