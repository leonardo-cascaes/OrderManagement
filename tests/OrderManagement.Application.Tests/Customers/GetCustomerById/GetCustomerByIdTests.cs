using FluentAssertions;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Customers.Queries.GetCustomerById;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Tests.Customers.GetCustomerById;

public class GetCustomerByIdTests
{
    [Fact]
    public async Task Should_return_customer_when_customer_exists()
    {
        // Arrange
        var customer = new Customer("Leonardo", "leonardo@email.com");

        var repository = new CustomerRepositoryStub(customer);

        var handler = new GetCustomerByIdHandler(repository);

        var query = new GetCustomerByIdQuery(customer.Id);

        // Act
        var result = await handler.Handle(
            query,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        result.Value.Id.Should().Be(customer.Id);
        result.Value.Name.Should().Be(customer.Name);
        result.Value.Email.Should().Be(customer.Email);
    }

    [Fact]
    public async Task Should_return_not_found_when_customer_does_not_exist()
    {
        // Arrange
        var repository = new CustomerRepositoryStub();

        var handler = new GetCustomerByIdHandler(repository);

        var query = new GetCustomerByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(
            query,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Customer not found.");
        result.ErrorType.Should().Be(ResultErrorType.NotFound);
    }

    private sealed class CustomerRepositoryStub : ICustomerRepository
    {
        private readonly Customer? _customer;

        public CustomerRepositoryStub(Customer? customer = null)
        {
            _customer = customer;
        }
        
        public void Add(Customer customer)
        {
        }

        public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_customer?.Id == id ? _customer : null);
        }

        public Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Customer>>(Array.Empty<Customer>());
        }
    }
}