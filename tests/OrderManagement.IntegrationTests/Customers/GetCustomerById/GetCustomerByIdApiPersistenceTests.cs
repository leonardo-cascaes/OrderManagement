using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Api.Models.Customers;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.TestSupport.Factories;
using System.Net;
using System.Net.Http.Json;

namespace OrderManagement.IntegrationTests.Customers.GetCustomerById;

public class GetCustomerByIdApiPersistenceTests
{
    [Fact]
    public async Task Should_return_customer_when_customer_exists()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        using var factory = new SqliteWebApplicationFactory();

        var customer = new Customer("Leonardo", "leonardo@email.com");

        await using (var arrangeScope = factory.Services.CreateAsyncScope())
        {
            var context = arrangeScope.ServiceProvider
                .GetRequiredService<OrderManagementDbContext>();

            await context.Database.EnsureCreatedAsync(cancellationToken);

            context.Customers.Add(customer);

            await context.SaveChangesAsync(cancellationToken);
        }

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/customers/{customer.Id}", cancellationToken);

        var responseCustomer = await response.Content
            .ReadFromJsonAsync<CustomerResponse>(cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        responseCustomer.Should().NotBeNull();
        responseCustomer.Id.Should().Be(customer.Id);
        responseCustomer.Name.Should().Be(customer.Name);
        responseCustomer.Email.Should().Be(customer.Email);
    }

    [Fact]
    public async Task Should_return_not_found_when_customer_does_not_exist()
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

        var customerId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/customers/{customerId}", cancellationToken);

        var content = await response.Content
            .ReadAsStringAsync(cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        content.Should().Contain("Customer not found.");
    }
}