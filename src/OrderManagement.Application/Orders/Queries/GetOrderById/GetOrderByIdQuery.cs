using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<Order>>;