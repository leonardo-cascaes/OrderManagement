using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Orders.Commands.CancelOrder;
using OrderManagement.Application.Persistence;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Tests.Orders.CancelOrder;

public class CancelOrderTests
{
    [Fact]
    public async Task Handle_ShouldCancelOrder_WhenOrderIsPending()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());

        var repository = new OrderRepositoryStub(order);
        var unitOfWork = new UnitOfWorkStub();

        var handler = new CancelOrderHandler(repository, unitOfWork);
        var command = new CancelOrderCommand(order.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var repository = new OrderRepositoryStub(null);
        var unitOfWork = new UnitOfWorkStub();

        var handler = new CancelOrderHandler(repository, unitOfWork);
        var command = new CancelOrderCommand(Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
        Assert.Equal("Order not found.", result.Error);
        Assert.Equal(0, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenOrderIsNotPending()
    {
        // Arrange
        var order = new Order(Guid.NewGuid());
        order.Confirm();

        var repository = new OrderRepositoryStub(order);
        var unitOfWork = new UnitOfWorkStub();

        var handler = new CancelOrderHandler(repository, unitOfWork);
        var command = new CancelOrderCommand(order.Id);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultErrorType.Conflict, result.ErrorType);
        Assert.Equal("Only pending orders can be cancelled.", result.Error);
        Assert.Equal(OrderStatus.Confirmed, order.Status);
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
            if (order is null)
            {
                return Task.FromResult<IReadOnlyCollection<Order>>(Array.Empty<Order>());
            }

            return Task.FromResult<IReadOnlyCollection<Order>>(new[] { order });
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