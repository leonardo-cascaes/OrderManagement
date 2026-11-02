using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Persistence.Repositories;

namespace OrderManagement.IntegrationTests.Infrastructure.Persistence.Repositories
{
    public class CustomerRepositoryTests
    {
        [Fact]
        public async Task Should_return_customer_when_customer_exists()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;

            await using var connection = new SqliteConnection("Data Source=:memory:");

            await connection.OpenAsync(cancellationToken);

            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite(connection)
                .Options;

            var customer = new Customer("Leonardo", "leonardo@email.com");

            await using (var arrangeContext = new OrderManagementDbContext(options))
            {
                await arrangeContext.Database.EnsureCreatedAsync(cancellationToken);

                arrangeContext.Customers.Add(customer);

                await arrangeContext.SaveChangesAsync(cancellationToken);
            }

            // Act
            await using var actContext = new OrderManagementDbContext(options);

            var repository = new CustomerRepository(actContext);

            var result = await repository.GetByIdAsync(customer.Id, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(customer.Id);
            result.Name.Should().Be(customer.Name);
            result.Email.Should().Be(customer.Email);
        }

        [Fact]
        public async Task Should_return_null_when_customer_does_not_exist()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;

            await using var connection = new SqliteConnection("Data Source=:memory:");

            await connection.OpenAsync(cancellationToken);

            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite(connection)
                .Options;

            await using (var arrangeContext = new OrderManagementDbContext(options))
            {
                await arrangeContext.Database.EnsureCreatedAsync(cancellationToken);
            }

            await using var actContext = new OrderManagementDbContext(options);

            var repository = new CustomerRepository(actContext);

            // Act
            var result = await repository.GetByIdAsync(Guid.NewGuid(), cancellationToken);

            // Assert
            result.Should().BeNull();
        }
    }
}
