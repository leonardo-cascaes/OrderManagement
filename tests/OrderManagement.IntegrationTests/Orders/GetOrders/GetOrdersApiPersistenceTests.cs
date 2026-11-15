using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Api.Models.Orders;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.TestSupport.Factories;

namespace OrderManagement.IntegrationTests.Orders.GetOrders;

public class GetOrdersApiPersistenceTests
{
    [Fact]
    public async Task Get_ShouldReturnOrdersWithItems_WhenOrdersExist()
    {
        // Arrange
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        Guid firstOrderId;
        Guid secondOrderId;
        Guid firstProductId;
        Guid secondProductId;

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

            var firstCustomer = new Customer("John Doe", "john@example.com");

            var secondCustomer = new Customer("Jane Doe", "jane@example.com");

            var firstProduct = new Product("Mechanical Keyboard", 100m, 10);

            var secondProduct = new Product("Gaming Mouse", 50m, 20);

            var firstOrder = new Order(firstCustomer.Id);

            firstOrder.AddItem(firstProduct.Id, 2, 100m);
            firstOrder.AddItem(secondProduct.Id, 1, 50m);

            var secondOrder = new Order(secondCustomer.Id);

            secondOrder.AddItem(secondProduct.Id, 3, 50m);

            dbContext.Customers.AddRange(firstCustomer, secondCustomer);

            dbContext.Products.AddRange(firstProduct, secondProduct);

            dbContext.Orders.AddRange(firstOrder, secondOrder);

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            firstOrderId = firstOrder.Id;
            secondOrderId = secondOrder.Id;
            firstProductId = firstProduct.Id;
            secondProductId = secondProduct.Id;
        }

        // Act
        var response = await client.GetAsync("/api/orders", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var orders = await response.Content
            .ReadFromJsonAsync<List<OrderResponse>>(TestContext.Current.CancellationToken);

        Assert.NotNull(orders);
        Assert.Equal(2, orders.Count);

        var returnedFirstOrder = Assert.Single(orders,order => order.Id == firstOrderId);

        Assert.Equal(250m, returnedFirstOrder.Total);
        Assert.Equal(2, returnedFirstOrder.OrderItems.Count);

        Assert.Contains(
            returnedFirstOrder.OrderItems,
            item => item.ProductId == firstProductId
                && item.Quantity == 2
                && item.UnitPrice == 100m);

        Assert.Contains(
            returnedFirstOrder.OrderItems,
            item => item.ProductId == secondProductId
                && item.Quantity == 1
                && item.UnitPrice == 50m);

        var returnedSecondOrder = Assert.Single(orders, order => order.Id == secondOrderId);

        Assert.Equal(150m, returnedSecondOrder.Total);
        Assert.Single(returnedSecondOrder.OrderItems);
    }

    [Fact]
    public async Task Get_ShouldReturnEmptyCollection_WhenNoOrdersExist()
    {
        // Arrange
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/orders", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var orders = await response.Content
            .ReadFromJsonAsync<List<OrderResponse>>(TestContext.Current.CancellationToken);

        Assert.NotNull(orders);
        Assert.Empty(orders);
    }
}