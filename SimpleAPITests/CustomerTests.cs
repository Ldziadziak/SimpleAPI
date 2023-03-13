using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using SimpleAPI.Interfaces;
using SimpleAPI.Models;
using SimpleAPI.Services;

namespace SimpleAPITests;

public class CustomerServiceTests
{
    private readonly ILogger<CustomerService> _logger;
    private readonly IMapper _mapper;
    public CustomerServiceTests()
    {
        var loggerMock = new Mock<ILogger<CustomerService>>();
        _logger = loggerMock.Object;

        var mapperMock = new Mock<IMapper>();
        _mapper = mapperMock.Object;
    }

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
        var customerService = new CustomerService(customerStoreMock.Object, _logger);

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
        var customerService = new CustomerService(mockCustomerStore.Object, _logger);
        var customers = new List<Customer> {
    new Customer {
      Id = 1, Name = "John", Surname = "Doe"
    },
    new Customer {
      Id = 2, Name = "John", Surname = "Doe"
    }
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
        var customer = new SimpleAPI.Entities.Customer()
        {
            Id = 1,
            Name = "John",
            Surname = "Doe"
        };

        var customerService = new CustomerService(mockCustomerStore.Object, _logger);
        var customerId = 1;

        // Mock the behavior of the GetByIdAsync method to return the added customer
        mockCustomerStore.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync(customer);

        // Act
        await customerService.AddCustomerAsync(_mapper.Map<Customer>(customer));
        await customerService.DeleteCustomerAsync(customerId);

        // Assert
        mockCustomerStore.Verify(x => x.DeleteAsync(customerId), Times.Once);
    }
    [Fact]
    public async Task DeleteCustomerAsync_NonExistingCustomerId_Failure()
    {
        // Arrange
        var mockCustomerStore = new Mock<ICustomerStore>();
        var customerService = new CustomerService(mockCustomerStore.Object, _logger);
        var customerId = 1;

        // Mock the behavior of the GetByIdAsync method to throw an EntityNotFoundException
        mockCustomerStore.Setup(x => x.GetByIdAsync(customerId)).Throws(new EntityNotFoundException(customerId, "abc"));

        // Act
        var result = await customerService.DeleteCustomerAsync(customerId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains(ICustomerService.NotFoundErrorCode, result.Errors.Select(e => e.Code));
        mockCustomerStore.Verify(x => x.DeleteAsync(customerId), Times.Never);
    }

    [Fact]
    public async Task DeleteCustomerAsync_StoreThrowsException_Failure()
    {
        // Arrange
        var mockCustomerStore = new Mock<ICustomerStore>();
        var customerService = new CustomerService(mockCustomerStore.Object, _logger);
        var customerId = 1;

        // Mock the behavior of the GetByIdAsync method to return a customer with the given id
        mockCustomerStore.Setup(x => x.GetByIdAsync(customerId)).ReturnsAsync(new SimpleAPI.Entities.Customer()
        {
            Id = customerId
        });

        // Mock the behavior of the DeleteAsync method to throw an exception
        mockCustomerStore.Setup(x => x.DeleteAsync(customerId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await customerService.DeleteCustomerAsync(customerId);

        // Assert
        Assert.False(result.Succeeded);
        Assert.Contains(ICustomerService.DbErrorCode, result.Errors.Select(e => e.Code));
        mockCustomerStore.Verify(x => x.DeleteAsync(customerId), Times.Once);
    }
}