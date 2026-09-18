using FluentAssertions;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Tests.Orders
{
    public class CreateOrderTests
    {
        [Fact]
        public async Task Should_not_create_order_when_customer_does_not_exist()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            var customerRepository = new CustomerRepositoryStub();

            var handler = CreateHandler(customerRepository);

            var command = new CreateOrderCommand(customerId, Guid.NewGuid(), 1);

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Customer not found.");
        }

        [Fact]
        public async Task Should_create_order_when_customer_exists()
        {
            // Arrange
            var customer = new Customer("Leonardo", "leonardo@email.com");
            var product = new Product("Notebook", 3500m, 10);

            var customerRepository = new CustomerRepositoryStub(customer);
            var productRepository = new ProductRepositoryStub(product);

            var handler = CreateHandler(customerRepository, productRepository);

            var command = new CreateOrderCommand(customer.Id, Guid.NewGuid(), 1);

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value!.CustomerId.Should().Be(customer.Id);
            result.Value.Status.Should().Be(OrderStatus.Pending);
        }

        [Fact]
        public async Task Should_not_create_order_when_product_does_not_exist()
        {
            // Arrange
            var customer = new Customer("Leonardo", "leonardo@email.com");

            var productId = Guid.NewGuid();

            var customerRepository = new CustomerRepositoryStub(customer);
            var productRepository = new ProductRepositoryStub();

            var handler = CreateHandler(customerRepository, productRepository);

            var command = new CreateOrderCommand(customer.Id, productId, 2);

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);


            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Product not found.");
        }

        private static CreateOrderHandler CreateHandler(
            ICustomerRepository? customerRepository = null,
            IProductRepository? productRepository = null
        )
        {
            customerRepository ??= new CustomerRepositoryStub();
            productRepository ??= new ProductRepositoryStub();

            return new CreateOrderHandler(
                customerRepository,
                productRepository);
        }

        private sealed class CustomerRepositoryStub : ICustomerRepository
        {
            private readonly Customer? _customer;

            public CustomerRepositoryStub(Customer? customer = null)
            {
                _customer = customer;
            }

            public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(_customer);
            }
        }

        private sealed class ProductRepositoryStub : IProductRepository
        {
            private readonly Product? _product;

            public ProductRepositoryStub(Product? product = null)
            {
                _product = product;
            }

            public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(_product);
            }
        }
    }
}
