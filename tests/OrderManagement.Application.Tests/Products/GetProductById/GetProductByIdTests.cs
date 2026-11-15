using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

// Adicione aqui o mesmo using utilizado pelo Handler
// para acessar ResultErrorType.

namespace OrderManagement.Application.Tests.Products.GetProductById;

public class GetProductByIdTests
{
    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        var product = new Product("Mechanical Keyboard", 299.90m, 10);

        var repository = new ProductRepositoryStub(product);

        var handler = new GetProductByIdHandler(repository);

        var query = new GetProductByIdQuery(product.Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(product.Id, result.Value!.Id);
        Assert.Equal("Mechanical Keyboard", result.Value.Name);
        Assert.Equal(299.90m, result.Value.Price);
        Assert.Equal(10, result.Value.Stock);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var repository = new ProductRepositoryStub(null);

        var handler = new GetProductByIdHandler(repository);

        var query = new GetProductByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
        Assert.Equal("Product not found.", result.Error);
    }

    private sealed class ProductRepositoryStub(Product? product) : IProductRepository
    {
        public void Add(Product product)
        {
            throw new NotSupportedException();
        }

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = product?.Id == id ? product : null;

            return Task.FromResult(result);
        }

        public Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Product>>(Array.Empty<Product>());
        }
    }
}