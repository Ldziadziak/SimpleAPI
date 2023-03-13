using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SimpleAPI.Controllers;
using SimpleAPI.DbContexts;
using SimpleAPI.Interfaces;
using SimpleAPI.Models;

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

    public async Task<Customer> AddCustomerAsync(Customer customer)
    {
        var entity = _mapper.Map<Entities.Customer>(customer);
        _context.Add(entity);
        await _context.SaveChangesAsync();

        _mapper.Map<Entities.Customer, Customer>(entity, customer);  // customer.Id = entity.Id;

        return await Task.FromResult(customer);
    }

    public async Task<Entities.Customer> GetByIdAsync(int customerId)
    {
        var customer = await Task.FromResult(_context.customer.FirstOrDefault(c => c.Id == customerId)!);
        if (customer == null)
        {
            _logger.LogWarning($"Failed to get customer with id {customerId}");
            throw new EntityNotFoundException(customerId, $"Failed to get customer {customerId} from db");
        }

        return _mapper.Map<Customer>(customer);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var customers = await _context.customer.ToListAsync(); 
        return customers.Select(c => _mapper.Map<Customer>(c));
    }

    public async Task DeleteAsync(int customerId)
    {
        var customer = _context.customer.FirstOrDefault(c => c.Id == customerId);
        _context.customer.Remove(customer!);

        await Task.CompletedTask;
    }
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
