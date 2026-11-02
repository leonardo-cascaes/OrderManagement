using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString = configuration.GetConnectionString("OrderManagement")
                ?? throw new InvalidOperationException("Connection string 'OrderManagement' was not found.");

            services.AddDbContext<OrderManagementDbContext>(options => options.UseNpgsql(connectionString));

            return services;
        }
    }
}
