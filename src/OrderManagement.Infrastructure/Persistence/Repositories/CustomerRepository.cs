using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly OrderManagementDbContext _context;

        public CustomerRepository(OrderManagementDbContext context)
        {
            _context = context;
        }

        public void Add(Customer customer)
        {
            _context.Customers.Add(customer);
        }

        public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default
        )
        {
            return await _context.Customers
                .FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyCollection<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Customers
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
