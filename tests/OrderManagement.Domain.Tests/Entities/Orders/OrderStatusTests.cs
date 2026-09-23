using FluentAssertions;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Domain.Tests.Entities.Orders
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

        [Fact]
        public void Should_cancel_pending_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            // Act
            order.Cancel();

            // Assert
            order.Status.Should().Be(OrderStatus.Cancelled);
        }

        [Fact]
        public void Should_complete_confirmed_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            order.Confirm();

            // Act
            order.Complete();

            // Assert
            order.Status.Should().Be(OrderStatus.Completed);
        }

        [Fact]
        public void Should_not_complete_pending_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            // Act
            Action act = () => order.Complete();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_not_confirm_already_confirmed_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            order.Confirm();

            // Act
            Action act = () => order.Confirm();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_not_cancel_confirmed_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            order.Confirm();

            // Act
            Action act = () => order.Cancel();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_not_confirm_cancelled_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Cancel();

            // Act
            Action act = () => order.Confirm();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_not_cancel_cancelled_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Cancel();

            // Act
            Action act = () => order.Cancel();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_not_complete_cancelled_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Cancel();

            // Act
            Action act = () => order.Complete();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_not_confirm_completed_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Confirm();
            order.Complete();

            // Act
            Action act = () => order.Confirm();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_not_cancel_completed_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Confirm();
            order.Complete();

            // Act
            Action act = () => order.Cancel();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_not_complete_completed_order()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.Confirm();
            order.Complete();

            // Act
            Action act = () => order.Complete();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
