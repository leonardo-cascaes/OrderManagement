using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.ConfirmOrder;

public record ConfirmOrderCommand(Guid Id) : IRequest<Result<Order>>;