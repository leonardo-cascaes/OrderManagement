using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.IntegrationTests.TestSupport.Stubs
{
    public class ProductRepositoryStub : IProductRepository
    {
        private readonly Product? _product;

        public ProductRepositoryStub(Product? product = null)
        {
            _product = product;
        }

        public void Add(Product product)
        {
        }

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_product);
        }

        public Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Product>>(Array.Empty<Product>());
        }
    }
}
