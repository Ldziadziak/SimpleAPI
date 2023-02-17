using Moq;
using SimpleAPI.Data;
using SimpleAPI.Models;
using SimpleAPI.Services;

namespace SimpleAPITests
{
    public class CustomerServiceTests
    {
        [Fact]
        public async Task AddCustomerAsync_ValidCustomer_ReturnsCreatedCustomer()
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

            // Set up the mock to return the added customer for the GetByIdAsync method
            mockCustomerStore.Setup(x => x.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

            // Act
            var addedCustomer = await customerService.AddCustomerAsync(customer);

            // Assert
            // Check that the added customer is the same as the expected customer
            Assert.Equal(customer, addedCustomer);
            // Verify that the CreateAsync method was called once with the expected customer
            mockCustomerStore.Verify(x => x.CreateAsync(customer), Times.Once);
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