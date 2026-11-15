using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, Result<IReadOnlyCollection<Order>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<IReadOnlyCollection<Order>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);

        return Result<IReadOnlyCollection<Order>>.Success(orders);
    }
}