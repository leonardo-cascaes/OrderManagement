using FluentAssertions;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Tests.Entities
{
    public class OrderTests
    {
        [Fact]
        public void Should_create_order_as_pending()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            // Act
            var order = new Order(customerId);

            // Assert
            order.Status.Should().Be(OrderStatus.Pending);
        }
    }
}
