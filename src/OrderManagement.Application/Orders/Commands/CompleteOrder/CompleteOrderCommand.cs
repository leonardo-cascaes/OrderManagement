using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.CompleteOrder;

public record CompleteOrderCommand(Guid Id) : IRequest<Result<Order>>;