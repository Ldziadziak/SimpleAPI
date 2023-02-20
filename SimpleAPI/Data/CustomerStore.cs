using SimpleAPI.Models;

namespace SimpleAPI.Data
{
    public class InMemoryCustomerStore : ICustomerStore
    {
        private readonly List<Customer> _customers = new List<Customer>();

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            if (customer.Id == 0)
            {
                customer.Id = _customers.Count + 1;
            }
            _customers.Add(customer);
            return await Task.FromResult(customer);
        }

        public async Task<Customer> GetByIdAsync(int customerId)
        {
            return await Task.FromResult(_customers.FirstOrDefault(c => c.Id == customerId)!);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await Task.FromResult(_customers.AsEnumerable());
        }

        public async Task DeleteAsync(int customerId)
        {
            var customer = _customers.FirstOrDefault(c => c.Id == customerId);
            if (customer != null)
            {
                _customers.Remove(customer);
            }
            await Task.CompletedTask;
        }
    }

}
