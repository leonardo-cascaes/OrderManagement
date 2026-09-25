using FluentValidation.TestHelper;
using OrderManagement.Application.Orders.Commands.CreateOrder;

namespace OrderManagement.Application.Tests.Orders.CreateOrder
{
    public class CreateOrderValidatorTests
    {
        private readonly CreateOrderValidator _validator = new();

        [Fact]
        public void Should_have_error_when_quantity_is_zero()
        {
            // Arrange
            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), 0);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_have_error_when_quantity_is_negative()
        {
            // Arrange
            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), -1);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_not_have_error_when_quantity_is_valid()
        {
            // Arrange
            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), 1);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        }
    }
}
