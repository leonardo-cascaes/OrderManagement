using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.TestSupport.Factories;

namespace OrderManagement.IntegrationTests.Products.CreateProduct;

public class CreateProductApiPersistenceTests
{
    [Fact]
    public async Task Post_ShouldCreateProduct_AndPersistIt()
    {
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new
        {
            Name = "Mechanical Keyboard",
            Price = 299.90m,
            Stock = 10
        };

        var response = await client.PostAsJsonAsync("/api/products", request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        using var jsonDocument = JsonDocument.Parse(responseContent);

        var productId = jsonDocument.RootElement
            .GetProperty("id")
            .GetGuid();

        await using var scope = factory.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<OrderManagementDbContext>();

        var product = await dbContext.Products
            .AsNoTracking()
            .SingleAsync(product => product.Id == productId, TestContext.Current.CancellationToken);

        Assert.Equal("Mechanical Keyboard", product.Name);
        Assert.Equal(299.90m, product.Price);
        Assert.Equal(10, product.Stock);
    }
}