using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Orders.Queries.GetOrderById;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Tests.Orders.GetOrderById;

public class GetOrderByIdTests
{
    [Fact]
    public async Task Handle_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var order = new Order(customerId);
        order.AddItem(productId, 2, 100m);

        var repository = new OrderRepositoryStub(order);
        var handler = new GetOrderByIdHandler(repository);
        var query = new GetOrderByIdQuery(order.Id);

        // Act
        var result = await handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Equal(order.Id, result.Value.Id);
        Assert.Equal(customerId, result.Value.CustomerId);
        Assert.Equal(200m, result.Value.Total);

        Assert.Single(result.Value.OrderItems);

        var item = result.Value.OrderItems.Single();

        Assert.Equal(productId, item.ProductId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(100m, item.UnitPrice);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var repository = new OrderRepositoryStub(null);
        var handler = new GetOrderByIdHandler(repository);

        var query = new GetOrderByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(
            query,
            CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
        Assert.Equal("Order not found.", result.Error);
    }

    private sealed class OrderRepositoryStub(Order? order) : IOrderRepository
    {
        public void Add(Order order)
        {
            throw new NotSupportedException();
        }

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var result = order?.Id == id ? order : null;

            return Task.FromResult(result);
        }


        public Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyCollection<Order>>(Array.Empty<Order>());
        }
    }
}