using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Persistence.Repositories;

namespace OrderManagement.IntegrationTests.Infrastructure.Persistence.Repositories
{
    public class ProductRepositoryTests
    {
        [Fact]
        public async Task Should_return_product_when_product_exists()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;

            await using var connection = new SqliteConnection("Data Source=:memory:");

            await connection.OpenAsync(cancellationToken);

            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite(connection)
                .Options;

            var product = new Product("Notebook", 3500m, 10);

            await using (var arrangeContext = new OrderManagementDbContext(options))
            {
                await arrangeContext.Database.EnsureCreatedAsync(cancellationToken);

                arrangeContext.Products.Add(product);

                await arrangeContext.SaveChangesAsync(cancellationToken);
            }

            // Act
            await using var actContext = new OrderManagementDbContext(options);

            var repository = new ProductRepository(actContext);

            var result = await repository.GetByIdAsync(product.Id, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(product.Id);
            result.Name.Should().Be(product.Name);
            result.Price.Should().Be(product.Price);
            result.Stock.Should().Be(product.Stock);
        }

        [Fact]
        public async Task Should_return_null_when_product_does_not_exist()
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

            var repository = new ProductRepository(actContext);

            // Act
            var result = await repository.GetByIdAsync(Guid.NewGuid(), cancellationToken);

            // Assert
            result.Should().BeNull();
        }
    }
}
