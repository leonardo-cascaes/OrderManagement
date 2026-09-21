using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.IntegrationTests.Infrastructure;

namespace OrderManagement.IntegrationTests
{
    public class DependencyInjectionTests
        : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public DependencyInjectionTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void Should_resolve_mediatr()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();

            // Act
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            // Assert
            sender.Should().NotBeNull();
        }

        [Fact]
        public async Task Should_dispatch_create_order_command_to_handler()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();

            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), 1);

            // Act
            var result = await sender.Send(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Customer not found.");
        }
    }
}
