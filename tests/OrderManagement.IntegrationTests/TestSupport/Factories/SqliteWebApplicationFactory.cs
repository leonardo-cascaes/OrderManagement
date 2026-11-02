using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using OrderManagement.Infrastructure.Persistence;
using System.Data.Common;

namespace OrderManagement.IntegrationTests.TestSupport.Factories;

public class SqliteWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbContextOptionsConfiguration<OrderManagementDbContext>>();
            services.RemoveAll<DbConnection>();

            services.AddSingleton<DbConnection>(_ =>
            {
                var connection = new SqliteConnection("Data Source=:memory:");

                connection.Open();

                return connection;
            });

            services.AddDbContext<OrderManagementDbContext>(
                (provider, options) =>
                {
                    var connection = provider.GetRequiredService<DbConnection>();

                    options.UseSqlite(connection);
                });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(configuration =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        "ConnectionStrings:OrderManagement",
                        "Host=localhost;Port=5432;Database=ordermanagement;Username=test;Password=test"
                    }
                });
        });

        return base.CreateHost(builder);
    }
}