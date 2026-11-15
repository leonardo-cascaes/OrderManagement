using FluentAssertions;
using OrderManagement.Application.Persistence;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Products.Commands.CreateProduct;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Tests.Products.CreateProduct;

public class CreateProductTests
{
    [Fact]
    public async Task Should_create_product_and_save_changes()
    {
        // Arrange
        var productRepository = new ProductRepositoryStub();
        var unitOfWork = new UnitOfWorkStub();

        var handler = new CreateProductHandler(productRepository, unitOfWork);

        var command = new CreateProductCommand("Notebook", 3500m, 10);

        // Act
        var result = await handler.Handle(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        result.Value.Name.Should().Be("Notebook");
        result.Value.Price.Should().Be(3500m);
        result.Value.Stock.Should().Be(10);

        productRepository.AddedProduct.Should().BeSameAs(result.Value);
        productRepository.AddCallCount.Should().Be(1);

        unitOfWork.SaveChangesCallCount.Should().Be(1);
    }

    private sealed class ProductRepositoryStub : IProductRepository
    {
        public Product? AddedProduct { get; private set; }

        public int AddCallCount { get; private set; }

        public void Add(Product product)
        {
            AddedProduct = product;
            AddCallCount++;
        }

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Product?>(null);
        }

        public Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Product>>(Array.Empty<Product>());
        }
    }

    private sealed class UnitOfWorkStub : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;

            return Task.FromResult(1);
        }
    }
}