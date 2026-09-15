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

        [Fact]
        public void Should_not_create_order_without_customer()
        {
            // Arrange
            var customerId = Guid.Empty;

            // Act
            Action act = () => new Order(customerId);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_add_item_to_order()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            var order = new Order(customerId);

            // Act
            order.AddItem(productId, 2, 100m);

            // Assert
            order.OrderItems.Should().ContainSingle();

            var item = order.OrderItems.Single();

            item.ProductId.Should().Be(productId);
            item.Quantity.Should().Be(2);
            item.UnitPrice.Should().Be(100m);
        }

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
