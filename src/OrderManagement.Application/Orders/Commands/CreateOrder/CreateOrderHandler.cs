using MediatR;
using OrderManagement.Application.Common;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Persistence;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Exceptions;

namespace OrderManagement.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler
        : IRequestHandler<CreateOrderCommand, Result<Order>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderHandler(
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork
        )
        {
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Order>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
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

            _orderRepository.Add(order);

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (ConcurrencyException ex)
            {
                return Result<Order>.Failure(ex.Message, ResultErrorType.Conflict);
            }

            return Result<Order>.Success(order);
        }
    }
}
