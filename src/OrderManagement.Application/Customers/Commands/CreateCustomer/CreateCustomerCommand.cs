using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Customers.Commands.CreateCustomer
{
    public record CreateCustomerCommand(string Name, string Email) : IRequest<Result<Customer>>;
}
