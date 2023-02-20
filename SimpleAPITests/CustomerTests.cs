using Microsoft.AspNetCore.Identity;
using Moq;
using SimpleAPI.Data;
using SimpleAPI.Models;
using SimpleAPI.Services;

namespace SimpleAPITests
{
    public class CustomerServiceTests
    {
        [Fact]
        public async Task AddCustomerAsync_WhenCustomerIsValid_ReturnsIdentityResultSuccess()
        {
            // Arrange
            var customer = new Customer
            {
                Id = 1,
                Name = "John",
                Surname = "Doe"
            };

            // Create a mock ICustomerStore object that will be used to simulate adding a customer
            var customerStoreMock = new Mock<ICustomerStore>();

            // Set up the mock to return true when AddCustomerAsync is called with the customer object
            customerStoreMock.Setup(store => store.AddCustomerAsync(customer));

            // Create a new CustomerService object and inject the mock customer store into it
            var customerService = new CustomerService(customerStoreMock.Object);

            // Act
            var result = await customerService.AddCustomerAsync(customer);

            // Assert
            // Check that the result is an IdentityResult with the Success flag set
            Assert.Equal(IdentityResult.Success, result);

            // Verify that AddCustomerAsync was called exactly once with the correct customer object
            customerStoreMock.Verify(store => store.AddCustomerAsync(customer), Times.Once);
        }


        [Fact]
        public async Task GetAllCustomersAsync_ReturnsListOfCustomers()
        {
            // Arrange
            var mockCustomerStore = new Mock<ICustomerStore>();
            var customerService = new CustomerService(mockCustomerStore.Object);
            var customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "John", Surname = "Doe" },
            new Customer { Id = 2, Name = "John", Surname = "Doe" }
        };
            mockCustomerStore.Setup(x => x.GetAllAsync()).ReturnsAsync(customers);

            // Act
            var result = await customerService.GetAllCustomersAsync();

            // Assert
            Assert.Equal(customers, result);
            mockCustomerStore.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomerAsync_ExistingCustomerId_DeletesCustomer()
        {
            // Arrange
            var mockCustomerStore = new Mock<ICustomerStore>();
            var customer = new Customer()
            {
                Id = 1,
                Name = "John",
                Surname = "Doe"
            };

            var customerService = new CustomerService(mockCustomerStore.Object);
            var customerId = 1;

            // Mock the behavior of the GetByIdAsync method to return the added customer
            mockCustomerStore.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync(customer);

            // Act
            await customerService.AddCustomerAsync(customer);
            await customerService.DeleteCustomerAsync(customerId);

            // Assert
            mockCustomerStore.Verify(x => x.DeleteAsync(customerId), Times.Once);
        }


        [Fact]
        public async Task DeleteCustomerAsync_NonExistingCustomerId()
        {
            // Arrange
            var mockCustomerStore = new Mock<ICustomerStore>();
            Customer customer = null!;

            var customerService = new CustomerService(mockCustomerStore.Object);
            var customerId = 1;

            // Mock the behavior of the GetByIdAsync method to return the added customer
            mockCustomerStore.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync(customer);

            // Act
            await customerService.AddCustomerAsync(customer);
            await customerService.DeleteCustomerAsync(customerId);

            // Assert
            mockCustomerStore.Verify(x => x.DeleteAsync(customerId), Times.Never);
        }

    }
}