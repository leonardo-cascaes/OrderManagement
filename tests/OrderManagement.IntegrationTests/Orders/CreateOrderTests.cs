using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderManagement.Api.Models.Orders;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderManagement.IntegrationTests.Orders
{
    public class CreateOrderTests
        : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public CreateOrderTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Should_return_not_found_when_customer_does_not_exist()
        {
            // Arrange
            using var client = _factory.CreateClient();

            var request = new
            {
                customerId = Guid.NewGuid(),
                productId = Guid.NewGuid(),
                quantity = 1
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/orders", request, TestContext.Current.CancellationToken);

            var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Should().Contain("Customer not found.");
        }

        [Fact]
        public async Task Should_create_order_when_customer_and_product_exist()
        {
            // Arrange
            var customer = new Customer(
                "Leonardo",
                "leonardo@email.com");

            var product = new Product(
                "Notebook",
                3500m,
                10);

            using var factory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<ICustomerRepository>();
                    services.RemoveAll<IProductRepository>();

                    services.AddScoped<ICustomerRepository>(_ => new CustomerRepositoryStub(customer));

                    services.AddScoped<IProductRepository>(_ => new ProductRepositoryStub(product));
                });
            });

            using var client = factory.CreateClient();

            var request = new
            {
                customerId = customer.Id,
                productId = product.Id,
                quantity = 2
            };

            // Act
            var response = await client.PostAsJsonAsync($"/api/orders/", request, TestContext.Current.CancellationToken);

            var responseOrder = await response.Content.ReadFromJsonAsync<OrderResponse>(TestContext.Current.CancellationToken);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            responseOrder.Should().NotBeNull();

            responseOrder.CustomerId.Should().Be(customer.Id);
            responseOrder.Status.Should().Be(OrderStatus.Pending);
            responseOrder.Total.Should().Be(7000m);

            responseOrder.OrderItems.Should().ContainSingle();

            var item = responseOrder.OrderItems.Single();

            item.ProductId.Should().Be(product.Id);
            item.Quantity.Should().Be(2);
            item.UnitPrice.Should().Be(3500m);
        }
    }
}
