using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Api.Models.Products;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.TestSupport.Factories;

namespace OrderManagement.IntegrationTests.Products.GetProductById;

public class GetProductByIdApiPersistenceTests
{
    [Fact]
    public async Task Get_ShouldReturnProduct_WhenProductExists()
    {
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        Guid productId;

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider
                .GetRequiredService<OrderManagementDbContext>();

            var product = new Product("Mechanical Keyboard", 299.90m, 10);

            dbContext.Products.Add(product);

            await dbContext.SaveChangesAsync(
                TestContext.Current.CancellationToken);

            productId = product.Id;
        }

        var response = await client.GetAsync($"/api/products/{productId}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var productResponse = await response.Content
            .ReadFromJsonAsync<ProductResponse>(TestContext.Current.CancellationToken);

        Assert.NotNull(productResponse);
        Assert.Equal(productId, productResponse.Id);
        Assert.Equal("Mechanical Keyboard", productResponse.Name);
        Assert.Equal(299.90m, productResponse.Price);
        Assert.Equal(10, productResponse.Stock);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        var productId = Guid.NewGuid();

        var response = await client.GetAsync($"/api/products/{productId}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Contains("Product not found.", responseContent);
    }
}