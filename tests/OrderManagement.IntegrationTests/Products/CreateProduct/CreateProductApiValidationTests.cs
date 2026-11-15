using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Api.Models.Products;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.TestSupport.Factories;
using System.Net;
using System.Net.Http.Json;

namespace OrderManagement.IntegrationTests.Products.CreateProduct;

public class CreateProductApiValidationTests
{
    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        await AssertInvalidProductAsync(
            new CreateProductRequest("", 100m, 10));
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenNameContainsOnlyWhitespace()
    {
        await AssertInvalidProductAsync(
            new CreateProductRequest("   ", 100m, 10));
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenPriceIsZero()
    {
        await AssertInvalidProductAsync(
            new CreateProductRequest("Keyboard", 0m, 10));
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenPriceIsNegative()
    {
        await AssertInvalidProductAsync(
            new CreateProductRequest("Keyboard", -10m, 10));
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenStockIsNegative()
    {
        await AssertInvalidProductAsync(
            new CreateProductRequest("Keyboard", 100m, -1));
    }

    private static async Task AssertInvalidProductAsync(
        CreateProductRequest request)
    {
        using var factory = new SqliteWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/products",request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        await using var scope = factory.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<OrderManagementDbContext>();

        var productsCount = await dbContext.Products.CountAsync(TestContext.Current.CancellationToken);

        Assert.Equal(0, productsCount);
    }
}