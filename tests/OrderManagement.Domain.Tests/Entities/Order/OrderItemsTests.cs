using FluentAssertions;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Tests.Entities
{
    public class OrderItemsTests
    {
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
        public void Should_not_add_item_to_confirmed_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Confirm();

            // Act
            Action act = () => order.AddItem(
                Guid.NewGuid(),
                2,
                10m);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_not_add_item_to_cancelled_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Cancel();

            // Act
            Action act = () => order.AddItem(
                Guid.NewGuid(),
                2,
                10m);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_not_add_item_to_completed_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Confirm();
            order.Complete();

            // Act
            Action act = () => order.AddItem(
                Guid.NewGuid(),
                2,
                10m);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
