using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Persistence;

namespace OrderManagement.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ICustomerRepository>();
            services.RemoveAll<IProductRepository>();
            services.RemoveAll<IOrderRepository>();
            services.RemoveAll<IUnitOfWork>();

            services.AddScoped<ICustomerRepository, CustomerRepositoryStub>();
            services.AddScoped<IProductRepository, ProductRepositoryStub>();
            services.AddScoped<IOrderRepository, OrderRepositoryStub>();
            services.AddScoped<IUnitOfWork, UnitOfWorkStub>();
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