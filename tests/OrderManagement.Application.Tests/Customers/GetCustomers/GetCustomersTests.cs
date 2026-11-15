using FluentAssertions;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Customers.Queries.GetCustomers;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Tests.Customers.GetCustomers;

public class GetCustomersTests
{
    [Fact]
    public async Task Should_return_customers_when_customers_exist()
    {
        // Arrange
        var customers = new List<Customer>
        {
            new("Leonardo", "leonardo@email.com"),
            new("Maria", "maria@email.com")
        };

        var repository = new CustomerRepositoryStub(customers);

        var handler = new GetCustomersHandler(repository);

        var query = new GetCustomersQuery();

        // Act
        var result = await handler.Handle(
            query,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(customers);
    }

    [Fact]
    public async Task Should_return_empty_collection_when_no_customers_exist()
    {
        // Arrange
        var repository = new CustomerRepositoryStub();

        var handler = new GetCustomersHandler(repository);

        var query = new GetCustomersQuery();

        // Act
        var result = await handler.Handle(
            query,
            TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Should().BeEmpty();
    }

    private sealed class CustomerRepositoryStub : ICustomerRepository
    {
        private readonly IReadOnlyCollection<Customer> _customers;

        public CustomerRepositoryStub(
            IReadOnlyCollection<Customer>? customers = null)
        {
            _customers = customers ?? [];
        }

        public Task<IReadOnlyCollection<Customer>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_customers);
        }

        public Task<Customer?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var customer = _customers
                .FirstOrDefault(x => x.Id == id);

            return Task.FromResult(customer);
        }

        public void Add(Customer customer)
        {
        }
    }
}