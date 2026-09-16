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
    }
}
