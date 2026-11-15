using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Api.Models.Products;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.TestSupport.Factories;

namespace OrderManagement.IntegrationTests.Products.GetProducts;

public class GetProductsApiPersistenceTests
{
    [Fact]
    public async Task Get_ShouldReturnProducts_WhenProductsExist()
    {
        // Arrange
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        var products = new List<Product>
        {
            new("Mechanical Keyboard", 299.90m, 10),
            new("Gaming Mouse", 149.90m, 20)
        };

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider
                .GetRequiredService<OrderManagementDbContext>();

            dbContext.Products.AddRange(products);

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        // Act
        var response = await client.GetAsync("/api/products", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var productResponses = await response.Content
            .ReadFromJsonAsync<List<ProductResponse>>(TestContext.Current.CancellationToken);

        Assert.NotNull(productResponses);
        Assert.Equal(2, productResponses.Count);

        Assert.Contains(
            productResponses,
            product => product.Name == "Mechanical Keyboard"
                && product.Price == 299.90m
                && product.Stock == 10);

        Assert.Contains(
            productResponses,
            product => product.Name == "Gaming Mouse"
                && product.Price == 149.90m
                && product.Stock == 20);
    }

    [Fact]
    public async Task Get_ShouldReturnEmptyCollection_WhenNoProductsExist()
    {
        // Arrange
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/products", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var productResponses = await response.Content
            .ReadFromJsonAsync<List<ProductResponse>>(TestContext.Current.CancellationToken);

        Assert.NotNull(productResponses);
        Assert.Empty(productResponses);
    }
}