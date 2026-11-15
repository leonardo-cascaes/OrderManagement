using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdHandler
    : IRequestHandler<GetOrderByIdQuery, Result<Order>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<Order>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
        {
            return Result<Order>.Failure("Order not found.", ResultErrorType.NotFound);
        }

        return Result<Order>.Success(order);
    }
}