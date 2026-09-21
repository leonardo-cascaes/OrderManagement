using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(Guid CustomerId, Guid ProductId, int Quantity) : IRequest<Result<Order>>;
}
