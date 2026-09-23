using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
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
}