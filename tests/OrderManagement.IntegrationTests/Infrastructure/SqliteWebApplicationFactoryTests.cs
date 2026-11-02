using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Persistence;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Persistence.Repositories;

namespace OrderManagement.IntegrationTests.Infrastructure;

public class SqliteWebApplicationFactoryTests
{
    [Fact]
    public async Task Should_resolve_real_persistence_services_with_sqlite_provider()
    {
        // Arrange
        using var factory = new SqliteWebApplicationFactory();

        using var scope = factory.Services.CreateScope();

        // Act
        var context = scope.ServiceProvider
            .GetRequiredService<OrderManagementDbContext>();

        var customerRepository = scope.ServiceProvider
            .GetRequiredService<ICustomerRepository>();

        var productRepository = scope.ServiceProvider
            .GetRequiredService<IProductRepository>();

        var orderRepository = scope.ServiceProvider
            .GetRequiredService<IOrderRepository>();

        var unitOfWork = scope.ServiceProvider
            .GetRequiredService<IUnitOfWork>();

        // Assert
        context.Database.ProviderName.Should().Be("Microsoft.EntityFrameworkCore.Sqlite");

        customerRepository.Should().BeOfType<CustomerRepository>();
        productRepository.Should().BeOfType<ProductRepository>();
        orderRepository.Should().BeOfType<OrderRepository>();

        unitOfWork.Should().BeSameAs(context);

        var cancellationToken = TestContext.Current.CancellationToken;

        await context.Database.EnsureCreatedAsync(cancellationToken);
        (await context.Database.CanConnectAsync(cancellationToken)).Should().BeTrue();
    }
}