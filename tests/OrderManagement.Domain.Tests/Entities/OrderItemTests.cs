using FluentAssertions;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Tests.Entities
{
    public class OrderItemTests
    {
        [Fact]
        public void Should_not_create_order_item_with_zero_quantity()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            Action act = () => new OrderItem(productId, 0, 100m);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_not_create_order_item_with_negative_quantity()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            Action act = () => new OrderItem(productId, -1, 100m);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_create_order_item_with_positive_quantity()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            var item = new OrderItem(productId, 2, 100m);

            // Assert
            item.ProductId.Should().Be(productId);
            item.Quantity.Should().Be(2);
        }

        [Fact]
        public void Should_not_create_order_item_with_zero_unit_price()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            Action act = () => new OrderItem(productId, 1, 0m);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_not_create_order_item_with_negative_unit_price()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            Action act = () => new OrderItem(productId, 1, -10m);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_create_order_item_with_positive_unit_price()
        {
            // Arrange
            var productId = Guid.NewGuid();

            // Act
            var item = new OrderItem(productId, 1, 100m);

            // Assert
            item.ProductId.Should().Be(productId);
            item.UnitPrice.Should().Be(100m);
        }
    }
}
