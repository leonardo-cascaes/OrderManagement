using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Customers.Queries.GetCustomerById;

public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, Result<Customer>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<Customer>> Handle(GetCustomerByIdQuery query, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(query.CustomerId, cancellationToken);

        if (customer is null)
        {
            return Result<Customer>.Failure("Customer not found.", ResultErrorType.NotFound);
        }

        return Result<Customer>.Success(customer);
    }
}