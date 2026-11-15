using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.IntegrationTests.TestSupport.Stubs
{
    public class CustomerRepositoryStub : ICustomerRepository
    {
        private readonly Customer? _customer;

        public CustomerRepositoryStub(Customer? customer = null)
        {
            _customer = customer;
        }

        public void Add(Customer customer)
        {
        }

        public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_customer);
        }

        public Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Customer>>(Array.Empty<Customer>());
        }
    }
}
