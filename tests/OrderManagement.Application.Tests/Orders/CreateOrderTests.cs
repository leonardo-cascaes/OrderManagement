using FluentAssertions;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Tests.Orders
{
    public class CreateOrderTests
    {
        [Fact]
        public async Task Should_not_create_order_when_customer_does_not_exist()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            var repository = new CustomerRepositoryStub();
            var handler = new CreateOrderHandler(repository);

            var command = new CreateOrderCommand(customerId);

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Customer not found.");
        }

        [Fact]
        public async Task Should_create_order_when_customer_exists()
        {
            // Arrange
            var customer = new Customer(
                "Leonardo",
                "leonardo@email.com");

            var repository = new CustomerRepositoryStub(customer);
            var handler = new CreateOrderHandler(repository);

            var command = new CreateOrderCommand(customer.Id);

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.CustomerId.Should().Be(customer.Id);
            result.Value.Status.Should().Be(OrderStatus.Pending);
        }

        private sealed class CustomerRepositoryStub : ICustomerRepository
        {
            private readonly Customer? _customer;

            public CustomerRepositoryStub(Customer? customer = null)
            {
                _customer = customer;
            }

            public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(_customer);
            }
        }
    }
}
