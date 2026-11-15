using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        void Add(Product product);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
