using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Customers.Queries.GetCustomers;

public record GetCustomersQuery() : IRequest<Result<IReadOnlyCollection<Customer>>>;