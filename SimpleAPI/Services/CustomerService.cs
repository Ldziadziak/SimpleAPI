namespace SimpleAPI.Services
{
    using SimpleAPI.Data;
    using SimpleAPI.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerStore _clientStore;
        //private readonly ICustomerValidator _customerValidator

        public CustomerService(ICustomerStore clientStore)
        {
            _clientStore = clientStore;
            //_customerValidator = customerValidator
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            //use validacions
            if (customer != null)
            {
                await _clientStore.CreateAsync(customer);
            }
            return customer;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _clientStore.GetAllAsync();
        }

        public async Task DeleteCustomerAsync(int Id)
        {
            var customer = await _clientStore.GetByIdAsync(Id);
            if (customer != null)
            {
                await _clientStore.DeleteAsync(Id);
            }
        }
    }

}
