using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<Result<Customer>>;