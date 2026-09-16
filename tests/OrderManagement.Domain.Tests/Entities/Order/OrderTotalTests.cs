using FluentAssertions;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Tests.Entities
{
    public class OrderTotalTests
    {
        [Fact]
        public void Should_return_zero_when_order_has_no_items()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            // Act
            var total = order.Total;

            // Assert
            total.Should().Be(0m);
        }

        [Fact]
        public void Should_calculate_total_for_one_item()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var order = new Order(customerId);

            order.AddItem(productId, 2, 100m);

            // Act
            var total = order.Total;

            // Assert
            total.Should().Be(200m);
        }

        [Fact]
        public void Should_calculate_total_for_multiple_items()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            order.AddItem(Guid.NewGuid(), 2, 100m);
            order.AddItem(Guid.NewGuid(), 1, 50m);
            order.AddItem(Guid.NewGuid(), 3, 20m);

            // Act
            var total = order.Total;

            // Assert
            total.Should().Be(310m);
        }
    }
}
