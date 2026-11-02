using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Api.Persistence
{
    public sealed class OrderManagementDbContextFactory
        : IDesignTimeDbContextFactory<OrderManagementDbContext>
    {
        public OrderManagementDbContext CreateDbContext(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddUserSecrets<OrderManagementDbContextFactory>(
                optional: true);

            var connectionString = builder.Configuration.GetConnectionString("OrderManagement")
                ?? throw new InvalidOperationException("Connection string 'OrderManagement' was not found.");

            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new OrderManagementDbContext(options);
        }
    }
}
