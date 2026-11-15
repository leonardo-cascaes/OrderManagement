using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Orders.Commands.CompleteOrder;
using OrderManagement.Application.Persistence;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Tests.Orders.CompleteOrder;

public class CompleteOrderTests
{
    [Fact]
    public async Task Handle_ShouldCompleteOrder_WhenOrderIsConfirmed()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());
        order.Confirm();

        var repository = new OrderRepositoryStub(order);
        var unitOfWork = new UnitOfWorkStub();

        var handler = new CompleteOrderHandler(repository, unitOfWork);
        var command = new CompleteOrderCommand(order.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var repository = new OrderRepositoryStub(null);
        var unitOfWork = new UnitOfWorkStub();

        var handler = new CompleteOrderHandler(repository, unitOfWork);
        var command = new CompleteOrderCommand(Guid.NewGuid());

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
        Assert.Equal("Order not found.", result.Error);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenOrderIsNotConfirmed()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());

        var repository = new OrderRepositoryStub(order);
        var unitOfWork = new UnitOfWorkStub();

        var handler = new CompleteOrderHandler(repository, unitOfWork);
        var command = new CompleteOrderCommand(order.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultErrorType.Conflict, result.ErrorType);
        Assert.Equal("Only confirmed orders can be completed.", result.Error);

        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    private sealed class OrderRepositoryStub(Order? order) : IOrderRepository
    {
        public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(order?.Id == id ? order : null);
        }

        public Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyCollection<Order> orders = order is null ? Array.Empty<Order>() : new[] { order };

            return Task.FromResult(orders);
        }

        public void Add(Order order)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class UnitOfWorkStub : IUnitOfWork
    {
        public int SaveChangesCalls { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCalls++;

            return Task.FromResult(1);
        }
    }
}