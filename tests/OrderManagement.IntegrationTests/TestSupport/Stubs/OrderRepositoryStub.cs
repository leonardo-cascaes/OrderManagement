using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.IntegrationTests.TestSupport.Stubs
{
    internal class OrderRepositoryStub : IOrderRepository
    {
        public void Add(Order order)
        {
        }

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Order?>(null);
        }


        public Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Order>>(Array.Empty<Order>());
        }
    }
}
