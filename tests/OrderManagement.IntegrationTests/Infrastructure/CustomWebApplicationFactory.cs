using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderManagement.Application.Interfaces.Repositories;

namespace OrderManagement.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddScoped<ICustomerRepository, CustomerRepositoryStub>();

            services.AddScoped<IProductRepository, ProductRepositoryStub>();
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