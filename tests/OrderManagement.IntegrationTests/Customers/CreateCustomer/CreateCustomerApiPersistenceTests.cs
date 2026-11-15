using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.TestSupport.Factories;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderManagement.IntegrationTests.Customers.CreateCustomer;

public class CreateCustomerApiPersistenceTests
{
    [Fact]
    public async Task Should_create_customer_through_api_and_persist_customer()
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

        var request = new
        {
            name = "Leonardo",
            email = "leonardo@email.com"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/customers", request, cancellationToken);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var responseCustomer = await response.Content
            .ReadFromJsonAsync<JsonElement>(cancellationToken);

        var customerId = responseCustomer.GetProperty("id").GetGuid();

        responseCustomer.GetProperty("name").GetString()
            .Should().Be("Leonardo");

        responseCustomer.GetProperty("email").GetString()
            .Should().Be("leonardo@email.com");

        // Assert persistence
        await using var assertScope = factory.Services.CreateAsyncScope();

        var assertContext = assertScope.ServiceProvider
            .GetRequiredService<OrderManagementDbContext>();

        var persistedCustomer = await assertContext.Customers
            .SingleOrDefaultAsync(customer => customer.Id == customerId, cancellationToken);

        persistedCustomer.Should().NotBeNull();
        persistedCustomer.Name.Should().Be("Leonardo");
        persistedCustomer.Email.Should().Be("leonardo@email.com");
    }

    [Fact]
    public async Task Should_return_bad_request_when_customer_name_is_empty()
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

        var request = new
        {
            name = "",
            email = "leonardo@email.com"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/customers", request, cancellationToken);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert persistence
        await using var assertScope = factory.Services.CreateAsyncScope();

        var assertContext = assertScope.ServiceProvider
            .GetRequiredService<OrderManagementDbContext>();

        var customerCount = await assertContext.Customers.CountAsync(cancellationToken);

        customerCount.Should().Be(0);
    }

    [Fact]
    public async Task Should_return_bad_request_when_customer_email_is_invalid()
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

        var request = new
        {
            name = "Leonardo",
            email = "invalid-email"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/customers", request, cancellationToken);

        // Assert HTTP
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Assert persistence
        await using var assertScope = factory.Services.CreateAsyncScope();

        var assertContext = assertScope.ServiceProvider
            .GetRequiredService<OrderManagementDbContext>();

        var customerCount = await assertContext.Customers.CountAsync(cancellationToken);

        customerCount.Should().Be(0);
    }
}