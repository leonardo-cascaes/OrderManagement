using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Api.Models.Orders;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.TestSupport.Factories;

namespace OrderManagement.IntegrationTests.Orders.GetOrderById;

public class GetOrderByIdApiPersistenceTests
{
    [Fact]
    public async Task Get_ShouldReturnOrderWithItems_WhenOrderExists()
    {
        // Arrange
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        Guid orderId;

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

            var customer = new Customer("John Doe", "john@example.com");

            var product = new Product("Mechanical Keyboard", 100m, 10);

            var order = new Order(customer.Id);

            order.AddItem(product.Id, 2, 100m);

            dbContext.Customers.Add(customer);
            dbContext.Products.Add(product);
            dbContext.Orders.Add(order);

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            orderId = order.Id;
        }

        // Act
        var response = await client.GetAsync($"/api/orders/{orderId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var orderResponse = await response.Content.ReadFromJsonAsync<OrderResponse>(TestContext.Current.CancellationToken);

        Assert.NotNull(orderResponse);

        Assert.Equal(orderId, orderResponse.Id);
        Assert.Equal(200m, orderResponse.Total);

        Assert.Single(orderResponse.OrderItems);

        var item = orderResponse.OrderItems.Single();

        Assert.Equal("Mechanical Keyboard", "Mechanical Keyboard");
        Assert.Equal(2, item.Quantity);
        Assert.Equal(100m, item.UnitPrice);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        var orderId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"/api/orders/{orderId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("Order not found.", responseContent);
    }
}