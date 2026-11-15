using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Orders.Commands.CancelOrder;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.IntegrationTests.TestSupport.Factories;

namespace OrderManagement.IntegrationTests.Orders.CancelOrder;

public class CancelOrderPersistenceTests
{
    [Fact]
    public async Task Handle_ShouldPersistCancelledStatus_WhenOrderIsPending()
    {
        // Arrange
        using var factory = new SqliteWebApplicationFactory();

        Guid orderId;

        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

            var customer = new Customer("John Doe", "john@example.com");

            var product = new Product("Mechanical Keyboard", 299.90m, 10);

            var order = new Order(customer.Id);

            order.AddItem(product.Id, 1, 299.90m);

            dbContext.Customers.Add(customer);
            dbContext.Products.Add(product);
            dbContext.Orders.Add(order);

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

            orderId = order.Id;
        }

        // Act
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var result = await sender.Send(new CancelOrderCommand(orderId), TestContext.Current.CancellationToken);

            Assert.True(result.IsSuccess);
            Assert.Equal(OrderStatus.Cancelled, result.Value!.Status);
        }

        // Assert
        await using (var scope = factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();

            var persistedOrder = await dbContext.Orders
                .AsNoTracking()
                .SingleAsync(order => order.Id == orderId, TestContext.Current.CancellationToken);

            Assert.Equal(OrderStatus.Cancelled, persistedOrder.Status);
        }
    }
}