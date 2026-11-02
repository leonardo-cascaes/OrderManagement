using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Persistence;
using OrderManagement.Infrastructure.DependencyInjection;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Persistence.Repositories;

namespace OrderManagement.IntegrationTests.Infrastructure.DependencyInjection
{
    public class InfrastructureServiceExtensionsTests
    {
        [Fact]
        public void Should_resolve_unit_of_work_as_same_dbcontext_instance()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                {
                    "ConnectionStrings:OrderManagement",
                    "Host=localhost;Port=5432;Database=ordermanagement;Username=test;Password=test"
                }
                })
                .Build();

            var services = new ServiceCollection();

            services.AddInfrastructure(configuration);

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();

            // Act
            var context = scope.ServiceProvider
                .GetRequiredService<OrderManagementDbContext>();

            var unitOfWork = scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();

            // Assert
            unitOfWork.Should().BeSameAs(context);
        }

        [Fact]
        public void Should_register_repositories_and_unit_of_work()
        {
            // Arrange
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
            {
                "ConnectionStrings:OrderManagement",
                "Host=localhost;Port=5432;Database=ordermanagement;Username=test;Password=test"
            }
                })
                .Build();

            var services = new ServiceCollection();

            services.AddInfrastructure(configuration);

            using var provider = services.BuildServiceProvider();
            using var scope = provider.CreateScope();

            // Act
            var customerRepository = scope.ServiceProvider
                .GetRequiredService<ICustomerRepository>();

            var productRepository = scope.ServiceProvider
                .GetRequiredService<IProductRepository>();

            var orderRepository = scope.ServiceProvider
                .GetRequiredService<IOrderRepository>();

            var unitOfWork = scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();

            var context = scope.ServiceProvider
                .GetRequiredService<OrderManagementDbContext>();

            // Assert
            customerRepository.Should().BeOfType<CustomerRepository>();
            productRepository.Should().BeOfType<ProductRepository>();
            orderRepository.Should().BeOfType<OrderRepository>();

            unitOfWork.Should().BeSameAs(context);
        }
    }
}
