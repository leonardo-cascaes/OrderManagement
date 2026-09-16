using FluentAssertions;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Tests.Entities
{
    public class OrderStatusTests
    {
        [Fact]
        public void Should_confirm_pending_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            // Act
            order.Confirm();

            // Assert
            order.Status.Should().Be(OrderStatus.Confirmed);
        }
    }
}
