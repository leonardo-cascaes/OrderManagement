using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Api.Models.Orders;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.TestSupport.Factories;

namespace OrderManagement.IntegrationTests.Orders.ConfirmOrder;

public class ConfirmOrderApiTests
{
    [Fact]
    public async Task Post_ShouldConfirmOrder_WhenOrderIsPending()
    {
        // Arrange
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        Guid orderId;

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

            var customer = new Customer("John Doe", "john@example.com");

            var product = new Product("Mechanical Keyboard", 299.90m, 10);

            var order = new Order(customer.Id);

            order.AddItem(product.Id, 1, 299.90m);

            dbContext.Customers.Add(customer);
            dbContext.Products.Add(product);
            dbContext.Orders.Add(order);

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            orderId = order.Id;
        }

        // Act
        var response = await client.PostAsync($"/api/orders/{orderId}/confirm", null, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var orderResponse = await response.Content
            .ReadFromJsonAsync<OrderResponse>(TestContext.Current.CancellationToken);

        Assert.NotNull(orderResponse);
        Assert.Equal(orderId, orderResponse.Id);
        Assert.Equal(OrderStatus.Confirmed, orderResponse.Status);
        Assert.Equal(299.90m, orderResponse.Total);
        Assert.Single(orderResponse.OrderItems);

        // Verify persistence
        await using var verificationScope = factory.Services.CreateAsyncScope();

        var verificationContext = verificationScope.ServiceProvider
            .GetRequiredService<OrderManagementDbContext>();

        var persistedOrder = await verificationContext.Orders
            .AsNoTracking()
            .SingleAsync(order => order.Id == orderId, TestContext.Current.CancellationToken);

        Assert.Equal(OrderStatus.Confirmed, persistedOrder.Status);
    }

    [Fact]
    public async Task Post_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        var orderId = Guid.NewGuid();

        // Act
        var response = await client.PostAsync($"/api/orders/{orderId}/confirm", null, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("Order not found.", responseContent);
    }

    [Fact]
    public async Task Post_ShouldReturnConflict_WhenOrderIsNotPending()
    {
        // Arrange
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        Guid orderId;

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

            var customer = new Customer("John Doe", "john@example.com");

            var order = new Order(customer.Id);

            order.Confirm();

            dbContext.Customers.Add(customer);
            dbContext.Orders.Add(order);

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            orderId = order.Id;
        }

        // Act
        var response = await client.PostAsync($"/api/orders/{orderId}/confirm", null, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("Only pending orders can be confirmed.", responseContent);
    }
}