using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Api.Models.Customers;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.TestSupport.Factories;
using System.Net;
using System.Net.Http.Json;

namespace OrderManagement.IntegrationTests.Customers.GetCustomers;

public class GetCustomersApiPersistenceTests
{
    [Fact]
    public async Task Should_return_customers_when_customers_exist()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        using var factory = new SqliteWebApplicationFactory();

        var customers = new List<Customer>
        {
            new("Leonardo", "leonardo@email.com"),
            new("Maria", "maria@email.com")
        };

        await using (var arrangeScope = factory.Services.CreateAsyncScope())
        {
            var context = arrangeScope.ServiceProvider
                .GetRequiredService<OrderManagementDbContext>();

            await context.Database.EnsureCreatedAsync(cancellationToken);

            context.Customers.AddRange(customers);

            await context.SaveChangesAsync(cancellationToken);
        }

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/customers", cancellationToken);

        var responseCustomers = await response.Content
            .ReadFromJsonAsync<List<CustomerResponse>>(cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        responseCustomers.Should().NotBeNull();
        responseCustomers.Should().HaveCount(2);

        responseCustomers.Should().ContainEquivalentOf(
            new CustomerResponse(
                customers[0].Id,
                customers[0].Name,
                customers[0].Email));

        responseCustomers.Should().ContainEquivalentOf(
            new CustomerResponse(
                customers[1].Id,
                customers[1].Name,
                customers[1].Email));
    }

    [Fact]
    public async Task Should_return_empty_collection_when_no_customers_exist()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        using var factory = new SqliteWebApplicationFactory();

        await using (var arrangeScope = factory.Services.CreateAsyncScope())
        {
            var context = arrangeScope.ServiceProvider
                .GetRequiredService<OrderManagementDbContext>();

            await context.Database.EnsureCreatedAsync(cancellationToken);
        }

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/customers", cancellationToken);

        var responseCustomers = await response.Content
            .ReadFromJsonAsync<List<CustomerResponse>>(cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        responseCustomers.Should().NotBeNull();
        responseCustomers.Should().BeEmpty();
    }
}