using OrderManagement.Application.Common;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler
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

        public async Task<Result<Order>> Handle(CreateOrderCommand command, CancellationToken cancellationToken = default)
        {
            if (command.Quantity <= 0)
                return Result<Order>.Failure("Quantity must be greater than zero.");

            var customer = await _customerRepository.GetByIdAsync(command.CustomerId, cancellationToken);

            if (customer is null)
                return Result<Order>.Failure("Customer not found.");

            var product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);

            if (product is null)
                return Result<Order>.Failure("Product not found.");

            var order = new Order(customer.Id);

            product.RemoveStock(command.Quantity);

            order.AddItem(product.Id, command.Quantity, product.Price);

            return Result<Order>.Success(order);
        }
    }
}
