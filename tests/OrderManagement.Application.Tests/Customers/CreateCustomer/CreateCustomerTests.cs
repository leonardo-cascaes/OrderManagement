using FluentAssertions;
using OrderManagement.Application.Customers.Commands.CreateCustomer;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Persistence;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Tests.Customers.CreateCustomer
{
    public class CreateCustomerTests
    {
        [Fact]
        public async Task Should_create_customer_and_save_changes()
        {
            // Arrange
            var customerRepository = new CustomerRepositoryStub();
            var unitOfWork = new UnitOfWorkStub();

            var handler = new CreateCustomerHandler(customerRepository, unitOfWork);

            var command = new CreateCustomerCommand("Leonardo", "leonardo@email.com");

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            result.Value.Name.Should().Be("Leonardo");
            result.Value.Email.Should().Be("leonardo@email.com");

            customerRepository.AddedCustomer.Should().BeSameAs(result.Value);
            customerRepository.AddCallCount.Should().Be(1);

            unitOfWork.SaveChangesCallCount.Should().Be(1);
        }

        private sealed class CustomerRepositoryStub : ICustomerRepository
        {
            public Customer? AddedCustomer { get; private set; }

            public int AddCallCount { get; private set; }

            public void Add(Customer customer)
            {
                AddedCustomer = customer;
                AddCallCount++;
            }

            public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            {
                return Task.FromResult<Customer?>(null);
            }

            public Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
            {
                return Task.FromResult<IReadOnlyCollection<Customer>>(Array.Empty<Customer>());
            }
        }

        private sealed class UnitOfWorkStub : IUnitOfWork
        {
            public int SaveChangesCallCount { get; private set; }

            public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            {
                SaveChangesCallCount++;
                return Task.FromResult(1);
            }
        }
    }
}
