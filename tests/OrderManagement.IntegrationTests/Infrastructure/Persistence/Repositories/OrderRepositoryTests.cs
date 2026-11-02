using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Persistence.Repositories;

namespace OrderManagement.IntegrationTests.Infrastructure.Persistence.Repositories
{
    public class OrderRepositoryTests
    {
        [Fact]
        public void Should_add_order_to_context()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            using var context = new OrderManagementDbContext(options);

            var order = new Order(Guid.NewGuid());

            order.AddItem(Guid.NewGuid(), 2, 3500m);

            var repository = new OrderRepository(context);

            // Act
            repository.Add(order);

            // Assert
            context.Entry(order).State.Should().Be(EntityState.Added);

            context.Entry(order.OrderItems.Single()).State.Should().Be(EntityState.Added);
        }
    }
}
