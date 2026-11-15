using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Persistence;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerHandler
    : IRequestHandler<CreateCustomerCommand, Result<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Customer>> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = new Customer(command.Name, command.Email);

        _customerRepository.Add(customer);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Customer>.Success(customer);
    }
}