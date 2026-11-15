using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Orders.Queries.GetOrders;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Tests.Orders.GetOrders;

public class GetOrdersTests
{
    [Fact]
    public async Task Handle_ShouldReturnOrders_WhenOrdersExist()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        var firstProductId = Guid.NewGuid();
        var secondProductId = Guid.NewGuid();

        var firstOrder = new Order(customerId);
        firstOrder.AddItem(firstProductId, 2, 100m);
        firstOrder.AddItem(secondProductId, 1, 50m);

        var secondOrder = new Order(Guid.NewGuid());
        secondOrder.AddItem(Guid.NewGuid(), 3, 80m);

        var orders = new List<Order>
        {
            firstOrder,
            secondOrder
        };

        var repository = new OrderRepositoryStub(orders);
        var handler = new GetOrdersHandler(repository);
        var query = new GetOrdersQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);

        Assert.Contains(result.Value, order => order.Id == firstOrder.Id);

        Assert.Contains(
            result.Value,
            order => order.Id == secondOrder.Id);

        var returnedOrder = result.Value.Single(order => order.Id == firstOrder.Id);

        Assert.Equal(250m, returnedOrder.Total);
        Assert.Equal(2, returnedOrder.OrderItems.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyCollection_WhenNoOrdersExist()
    {
        // Arrange
        var repository = new OrderRepositoryStub(
            Array.Empty<Order>());

        var handler = new GetOrdersHandler(repository);
        var query = new GetOrdersQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    private sealed class OrderRepositoryStub(IReadOnlyCollection<Order> orders) : IOrderRepository
    {
        public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var order = orders.FirstOrDefault(
                order => order.Id == id);

            return Task.FromResult(order);
        }


        public Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(orders);
        }

        public void Add(Order order)
        {
            throw new NotSupportedException();
        }
    }
}