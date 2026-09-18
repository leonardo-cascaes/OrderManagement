using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler
    {
        private readonly ICustomerRepository _customerRepository;

        public CreateOrderHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<Result<Order>> Handle(CreateOrderCommand command, CancellationToken cancellationToken = default)
        {
            var customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);

            if (customer is null)
            {
                return Result<Order>.Failure("Customer not found.");
            }

            var order = new Order(customer.Id);

            return Result<Order>.Success(order);
        }
    }
}
