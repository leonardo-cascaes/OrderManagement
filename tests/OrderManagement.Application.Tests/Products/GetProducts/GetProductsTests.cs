using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Products.Queries.GetProducts;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Tests.Products.GetProducts;

public class GetProductsTests
{
    [Fact]
    public async Task Handle_ShouldReturnProducts_WhenProductsExist()
    {
        // Arrange
        var products = new List<Product>
        {
            new("Mechanical Keyboard", 299.90m, 10),
            new("Gaming Mouse", 149.90m, 20)
        };

        var repository = new ProductRepositoryStub(products);
        var handler = new GetProductsHandler(repository);

        var query = new GetProductsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);

        Assert.Contains(result.Value, product => product.Name == "Mechanical Keyboard");

        Assert.Contains(result.Value, product => product.Name == "Gaming Mouse");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyCollection_WhenNoProductsExist()
    {
        // Arrange
        var repository = new ProductRepositoryStub([]);
        var handler = new GetProductsHandler(repository);

        var query = new GetProductsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    private sealed class ProductRepositoryStub(
        IReadOnlyCollection<Product> products) : IProductRepository
    {
        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = products.FirstOrDefault(product => product.Id == id);

            return Task.FromResult(product);
        }

        public Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(products);
        }

        public void Add(Product product)
        {
            throw new NotSupportedException();
        }
    }
}