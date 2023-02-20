namespace SimpleAPI.Services
{
    using Microsoft.AspNetCore.Identity;
    using SimpleAPI.Data;
    using SimpleAPI.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerStore _customerStore;
        //private readonly ICustomerValidator _customerValidator

        public CustomerService(ICustomerStore clientStore)
        {
            _customerStore = clientStore;
            //_customerValidator = customerValidator
        }

        public async Task<IdentityResult> AddCustomerAsync(Customer customer)
        {
            //use validacions
            try
            {
                await _customerStore.AddCustomerAsync(customer);
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Code = ICustomerService.DbErrorCode, Description = ex.Message });
            }
            return IdentityResult.Success;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _customerStore.GetAllAsync();
        }

        public async Task DeleteCustomerAsync(int Id)
        {
            var customer = await _customerStore.GetByIdAsync(Id);
            if (customer != null)
            {
                await _customerStore.DeleteAsync(Id);
            }
        }
    }

}
