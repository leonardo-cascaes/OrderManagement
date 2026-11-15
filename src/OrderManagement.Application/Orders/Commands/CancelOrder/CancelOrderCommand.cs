using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.CancelOrder;

public record CancelOrderCommand(Guid Id) : IRequest<Result<Order>>;