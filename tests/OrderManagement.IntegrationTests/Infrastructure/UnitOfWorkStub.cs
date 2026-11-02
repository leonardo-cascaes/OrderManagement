using OrderManagement.Application.Persistence;

namespace OrderManagement.IntegrationTests.Infrastructure
{
    public class UnitOfWorkStub : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(1);
        }
    }
}
