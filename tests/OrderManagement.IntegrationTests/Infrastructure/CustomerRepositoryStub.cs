using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.IntegrationTests.Infrastructure
{
    public class CustomerRepositoryStub : ICustomerRepository
    {
        private readonly Customer? _customer;

        public CustomerRepositoryStub(Customer? customer = null)
        {
            _customer = customer;
        }

        public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_customer);
        }
    }
}
