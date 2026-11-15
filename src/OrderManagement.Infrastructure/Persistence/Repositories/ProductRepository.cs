using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly OrderManagementDbContext _context;

        public ProductRepository(OrderManagementDbContext context)
        {
            _context = context;
        }

        public void Add(Product product)
        {
            _context.Products.Add(product);
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyCollection<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
