using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler
        : IRequestHandler<CreateOrderCommand, Result<Order>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        public CreateOrderHandler(
            ICustomerRepository customerRepository,
            IProductRepository productRepository
        )
        {
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        public async Task<Result<Order>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            if (command.Quantity <= 0)
                return Result<Order>.Failure("Quantity must be greater than zero.", ResultErrorType.Validation);

            var customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);

            if (customer is null)
                return Result<Order>.Failure("Customer not found.", ResultErrorType.NotFound);

            var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);

            if (product is null)
                return Result<Order>.Failure("Product not found.", ResultErrorType.NotFound);

            var order = new Order(customer.Id);

            try
            {
                product.RemoveStock(command.Quantity);
            }
            catch (InsufficientStockException ex)
            {

                return Result<Order>.Failure(ex.Message, ResultErrorType.Conflict);
            }

            order.AddItem(product.Id, command.Quantity, product.Price);

            return Result<Order>.Success(order);
        }
    }
}
