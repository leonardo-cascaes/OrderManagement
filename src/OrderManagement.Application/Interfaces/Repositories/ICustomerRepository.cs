using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        void Add(Customer customer);
        Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
