using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Queries.GetOrders;

public record GetOrdersQuery() : IRequest<Result<IReadOnlyCollection<Order>>>;