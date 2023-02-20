namespace SimpleAPI.Services
{
    using Microsoft.AspNetCore.Identity;
    using SimpleAPI.Interfaces;
    using SimpleAPI.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerStore _customerStore;
        //private readonly ICustomerValidator _customerValidator

        public CustomerService(ICustomerStore customerStore)
        {
            _customerStore = customerStore;
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

        public async Task<IEnumerable<Customer?>> GetAllCustomersAsync()
        {
            var customers = await _customerStore.GetAllAsync();
            if (customers == null)
            {
                return null!;
            }

            return customers;
        }

        public async Task<IdentityResult> DeleteCustomerAsync(int Id)
        {
            var customer = new Customer();

            try
            {
                customer = await _customerStore.GetByIdAsync(Id);
            }
            catch (EntityNotFoundException ex)
            {
                return IdentityResult.Failed(new IdentityError() { Code = ICustomerService.NotFoundErrorCode, Description = ex.Message });
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Code = ICustomerService.DbErrorCode, Description = ex.Message });
            }

            // ensure we can delete customer i.e. no users exist
            try
            {
                await _customerStore.DeleteAsync(customer.Id);
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Code = ICustomerService.DbErrorCode, Description = ex.Message });
            }

            return IdentityResult.Success;
        }

    }

}
