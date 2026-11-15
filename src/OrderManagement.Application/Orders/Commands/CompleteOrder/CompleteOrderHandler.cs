using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Persistence;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.CompleteOrder;

public class CompleteOrderHandler : IRequestHandler<CompleteOrderCommand, Result<Order>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteOrderHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Order>> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken);

        if (order is null)
        {
            return Result<Order>.Failure("Order not found.", ResultErrorType.NotFound);
        }

        try
        {
            order.Complete();
        }
        catch (InvalidOperationException exception)
        {
            return Result<Order>.Failure(exception.Message, ResultErrorType.Conflict);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Order>.Success(order);
    }
}