using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderManagement.Api.Models.Orders;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Persistence;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.Infrastructure;
using OrderManagement.IntegrationTests.TestSupport.Factories;
using System.Net;
using System.Net.Http.Json;

namespace OrderManagement.IntegrationTests.Orders;

public class CreateOrderApiPersistenceTests
{
    [Fact]
    public async Task Should_create_order_through_api_and_persist_order_and_stock()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        using var factory = new SqliteWebApplicationFactory();

        var customer = new Customer("Leonardo", "leonardo@email.com");
        var product = new Product("Notebook", 3500m, 10);

        await using (var arrangeScope = factory.Services.CreateAsyncScope())
        {
            var context = arrangeScope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

            await context.Database.EnsureCreatedAsync(cancellationToken);

            context.Customers.Add(customer);
            context.Products.Add(product);

            await context.SaveChangesAsync(cancellationToken);
        }

        using var client = factory.CreateClient();

        var request = new
        {
            customerId = customer.Id,
            productId = product.Id,
            quantity = 3
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/orders", request, cancellationToken);

        var responseOrder = await response.Content.ReadFromJsonAsync<OrderResponse>(cancellationToken);

        // Assert HTTP response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        responseOrder.Should().NotBeNull();
        responseOrder.CustomerId.Should().Be(customer.Id);
        responseOrder.Status.Should().Be(OrderStatus.Pending);
        responseOrder.Total.Should().Be(10500m);

        responseOrder.OrderItems.Should().ContainSingle();

        // Assert persistence
        await using var assertScope = factory.Services.CreateAsyncScope();

        var assertContext = assertScope.ServiceProvider
            .GetRequiredService<OrderManagementDbContext>();

        var persistedProduct = await assertContext.Products
            .SingleAsync(x => x.Id == product.Id, cancellationToken);

        var persistedOrder = await assertContext.Orders
            .Include(x => x.OrderItems)
            .SingleAsync(x => x.Id == responseOrder.Id, cancellationToken);

        persistedProduct.Stock.Should().Be(7);

        persistedOrder.CustomerId.Should().Be(customer.Id);
        persistedOrder.Status.Should().Be(OrderStatus.Pending);
        persistedOrder.Total.Should().Be(10500m);

        var item = persistedOrder.OrderItems.Single();

        item.ProductId.Should().Be(product.Id);
        item.Quantity.Should().Be(3);
        item.UnitPrice.Should().Be(3500m);
    }

    [Fact]
    public async Task Should_return_conflict_when_stock_is_insufficient()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        using var factory = new SqliteWebApplicationFactory();

        var customer = new Customer("Leonardo", "leonardo@email.com");
        var product = new Product("Notebook", 3500m, 2);

        await using (var arrangeScope = factory.Services.CreateAsyncScope())
        {
            var context = arrangeScope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

            await context.Database.EnsureCreatedAsync(cancellationToken);

            context.Customers.Add(customer);
            context.Products.Add(product);

            await context.SaveChangesAsync(cancellationToken);
        }

        using var client = factory.CreateClient();

        var request = new
        {
            customerId = customer.Id,
            productId = product.Id,
            quantity = 3
        };

        // Act
        var response = await client.PostAsJsonAsync(
            "/api/orders",
            request,
            cancellationToken);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        // Assert persistence
        await using var assertScope = factory.Services.CreateAsyncScope();

        var assertContext = assertScope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

        var persistedProduct = await assertContext.Products.SingleAsync(x => x.Id == product.Id, cancellationToken);

        var orderCount = await assertContext.Orders.CountAsync(cancellationToken);

        persistedProduct.Stock.Should().Be(2);
        orderCount.Should().Be(0);
    }

    [Fact]
    public async Task Should_return_bad_request_when_quantity_is_invalid()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        using var factory = new SqliteWebApplicationFactory();

        var customer = new Customer("Leonardo", "leonardo@email.com");
        var product = new Product("Notebook", 3500m, 10);

        await using (var arrangeScope = factory.Services.CreateAsyncScope())
        {
            var context = arrangeScope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

            await context.Database.EnsureCreatedAsync(cancellationToken);

            context.Customers.Add(customer);
            context.Products.Add(product);

            await context.SaveChangesAsync(cancellationToken);
        }

        using var client = factory.CreateClient();

        var request = new
        {
            customerId = customer.Id,
            productId = product.Id,
            quantity = 0
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/orders", request, cancellationToken);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert persistence
        await using var assertScope = factory.Services.CreateAsyncScope();

        var assertContext = assertScope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

        var persistedProduct = await assertContext.Products
            .SingleAsync(x => x.Id == product.Id, cancellationToken);

        var orderCount = await assertContext.Orders.CountAsync(cancellationToken);

        persistedProduct.Stock.Should().Be(10);
        orderCount.Should().Be(0);
    }

    [Fact]
    public async Task Should_return_conflict_when_concurrency_exception_occurs()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        using var baseFactory = new SqliteWebApplicationFactory();

        using var factory = baseFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IUnitOfWork>();
                services.AddScoped<IUnitOfWork, ConcurrencyUnitOfWorkStub>();
            });
        });

        var customer = new Customer("Leonardo", "leonardo@email.com");
        var product = new Product("Notebook", 3500m, 10);

        await using (var arrangeScope = factory.Services.CreateAsyncScope())
        {
            var context = arrangeScope.ServiceProvider
                .GetRequiredService<OrderManagementDbContext>();

            await context.Database.EnsureCreatedAsync(cancellationToken);

            context.Customers.Add(customer);
            context.Products.Add(product);

            await context.SaveChangesAsync(cancellationToken);
        }

        using var client = factory.CreateClient();

        var request = new
        {
            customerId = customer.Id,
            productId = product.Id,
            quantity = 3
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/orders", request, cancellationToken);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        // Assert persistence
        await using var assertScope = factory.Services.CreateAsyncScope();

        var assertContext = assertScope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

        var persistedProduct = await assertContext.Products
            .SingleAsync(x => x.Id == product.Id, cancellationToken);

        var orderCount = await assertContext.Orders.CountAsync(cancellationToken);

        persistedProduct.Stock.Should().Be(10);
        orderCount.Should().Be(0);
    }

    private sealed class ConcurrencyUnitOfWorkStub : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.FromException<int>(
                new ConcurrencyException("Product stock was modified by another operation. Please try again.")
            );
        }
    }
}