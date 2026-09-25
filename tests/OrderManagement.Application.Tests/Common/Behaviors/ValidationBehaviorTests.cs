using FluentAssertions;
using FluentValidation;
using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Common.Behaviors;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Domain.Entities;


namespace OrderManagement.Application.Tests.Common.Behaviors
{
    public class ValidationBehaviorTests
    {
        [Fact]
        public async Task Should_not_call_handler_when_request_is_invalid()
        {
            // Arrange
            var validator = new CreateOrderValidator();

            var behavior = new ValidationBehavior<CreateOrderCommand, Result<Order>>(new[] { validator });

            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), 0);

            var handlerCalled = false;

            RequestHandlerDelegate<Result<Order>> next = _ =>
            {
                handlerCalled = true;

                return Task.FromResult(
                    Result<Order>.Success(new Order(Guid.NewGuid()))
                );
            };

            // Act
            Func<Task> act = () => behavior.Handle(command, next, TestContext.Current.CancellationToken);

            // Assert
            await act.Should().ThrowAsync<ValidationException>();

            handlerCalled.Should().BeFalse();
        }

        [Fact]
        public async Task Should_call_handler_when_request_is_valid()
        {
            // Arrange
            var validator = new CreateOrderValidator();

            var behavior =new ValidationBehavior<CreateOrderCommand, Result<Order>>(new[] { validator });

            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), 1);

            var handlerCalled = false;

            RequestHandlerDelegate<Result<Order>> next = _ =>
            {
                handlerCalled = true;

                return Task.FromResult(
                    Result<Order>.Success(new Order(command.CustomerId))
                );
            };

            // Act
            var result = await behavior.Handle(command, next, TestContext.Current.CancellationToken);

            // Assert
            handlerCalled.Should().BeTrue();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
        }
    }
}
