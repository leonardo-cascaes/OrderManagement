using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.IntegrationTests.Infrastructure.Persistence;

public class ProductConcurrencyTests
{
    [Fact]
    public async Task Should_throw_concurrency_exception_when_stock_was_modified_by_another_context()
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

        await using var firstContext = new OrderManagementDbContext(options);
        await using var secondContext = new OrderManagementDbContext(options);

        var firstProduct = await firstContext.Products
            .SingleAsync(x => x.Id == product.Id, cancellationToken);

        var secondProduct = await secondContext.Products
            .SingleAsync(x => x.Id == product.Id, cancellationToken);

        // Act
        firstProduct.RemoveStock(6);
        secondProduct.RemoveStock(5);

        await firstContext.SaveChangesAsync(cancellationToken);

        Func<Task> act = () => secondContext.SaveChangesAsync(cancellationToken);

        // Assert
        var exception = await act.Should().ThrowAsync<ConcurrencyException>();

        exception.Which.InnerException.Should().BeOfType<DbUpdateConcurrencyException>();

        await using var assertContext = new OrderManagementDbContext(options);

        var persistedProduct = await assertContext.Products
            .SingleAsync(x => x.Id == product.Id, cancellationToken);

        persistedProduct.Stock.Should().Be(4);
    }
}