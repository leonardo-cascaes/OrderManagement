using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Customers.Queries.GetCustomers;

public class GetCustomersHandler
    : IRequestHandler<GetCustomersQuery, Result<IReadOnlyCollection<Customer>>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomersHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<IReadOnlyCollection<Customer>>> Handle(GetCustomersQuery query, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);

        return Result<IReadOnlyCollection<Customer>>.Success(customers);
    }
}