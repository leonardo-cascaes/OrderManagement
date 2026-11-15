using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderManagementDbContext _context;

        public OrderRepository(OrderManagementDbContext context)
        {
            _context = context;
        }

        public void Add(Order order)
        {
            _context.Orders.Add(order);
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .AsTracking()
                .Include(order => order.OrderItems)
                .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(order => order.OrderItems)
                .ToListAsync(cancellationToken);
        }
    }
}
