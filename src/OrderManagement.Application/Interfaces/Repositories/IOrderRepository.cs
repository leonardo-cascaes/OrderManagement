using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        void Add(Order order);
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
