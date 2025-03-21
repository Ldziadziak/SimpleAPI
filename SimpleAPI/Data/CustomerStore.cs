using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Controllers;
using SimpleAPI.DbContexts;
using SimpleAPI.Interfaces;
using SimpleAPI.Models;
using SimpleAPI.Services;

namespace SimpleAPI.Data;

public class DbCustomerStore : ICustomerStore
{
  private readonly ILogger<CustomerController> _logger;
  private readonly CustomerContext _context;
  private readonly IMapper _mapper;

  public DbCustomerStore(ILogger<CustomerController> logger, CustomerContext context, IMapper mapper)
  {
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    _context = context ?? throw new ArgumentNullException(nameof(context));
    _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
  }

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
    var customer = await Task.FromResult(_context.Customers.FirstOrDefault(c => c.Id == customerId)!);
    if (customer == null)
    {
      _logger.LogWarning($"Failed to get customer with id {customerId}");
      throw new EntityNotFoundException(customerId, $"Failed to get customer {customerId} from db");
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
      collection.Where(c => c.Name.StartsWith(name));
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
    var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
    _context.Customers.Remove(customer!);

    await Task.CompletedTask;
  }
  public async Task<int> SaveChangesAsync()
  {
    return await _context.SaveChangesAsync();
  }
}
