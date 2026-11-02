using OrderManagement.Application.Persistence;

namespace OrderManagement.IntegrationTests.TestSupport.Stubs
{
    public class UnitOfWorkStub : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(1);
        }
    }
}
