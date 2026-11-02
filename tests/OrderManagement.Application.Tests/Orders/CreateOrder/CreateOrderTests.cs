using FluentAssertions;
using OrderManagement.Application.Common;
using OrderManagement.Application.Common.Exceptions;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Application.Persistence;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;

namespace OrderManagement.Application.Tests.Orders.CreateOrder
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

        [Fact]
        public async Task Should_create_order_with_product_item()
        {
            // Arrange
            var customer = new Customer("Leonardo", "leonardo@email.com");

            var product = new Product("Notebook", 3500m, 10);

            var customerRepository = new CustomerRepositoryStub(customer);

            var productRepository = new ProductRepositoryStub(product);

            var handler = CreateHandler(customerRepository, productRepository);

            var command = new CreateOrderCommand(customer.Id, product.Id, 2);

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            result.Value!.OrderItems.Should().ContainSingle();

            var item = result.Value.OrderItems.Single();

            item.ProductId.Should().Be(product.Id);
            item.Quantity.Should().Be(2);
            item.UnitPrice.Should().Be(3500m);
        }


        [Fact]
        public async Task Should_not_create_order_when_product_stock_is_insufficient()
        {
            // Arrange
            var customer = new Customer("Leonardo", "leonardo@email.com");

            var product = new Product("Notebook", 3500m, 5);

            var customerRepository = new CustomerRepositoryStub(customer);

            var productRepository = new ProductRepositoryStub(product);

            var handler = CreateHandler(customerRepository, productRepository);

            var command = new CreateOrderCommand(customer.Id, product.Id, 6);

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Insufficient stock.");
            result.ErrorType.Should().Be(ResultErrorType.Conflict);
        }

        [Fact]
        public async Task Should_decrease_product_stock_when_order_is_created()
        {
            // Arrange
            var customer = new Customer("Leonardo", "leonardo@email.com");

            var product = new Product("Notebook", 3500m, 10);

            var customerRepository = new CustomerRepositoryStub(customer);
            
            var productRepository = new ProductRepositoryStub(product);

            var handler = CreateHandler(customerRepository, productRepository);

            var command = new CreateOrderCommand(customer.Id, product.Id, 3);

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            product.Stock.Should().Be(7);
        }

        [Fact]
        public async Task Should_add_order_and_save_changes_when_order_is_created()
        {
            // Arrange
            var customer = new Customer("Leonardo", "leonardo@email.com");
            var product = new Product("Notebook", 3500m, 10);

            var customerRepository = new CustomerRepositoryStub(customer);
            var productRepository = new ProductRepositoryStub(product);
            var orderRepository = new OrderRepositoryStub();
            var unitOfWork = new UnitOfWorkStub();

            var handler = CreateHandler(customerRepository, productRepository, orderRepository, unitOfWork);

            var command = new CreateOrderCommand(customer.Id, product.Id, 3);

            // Act
            var result = await handler.Handle(
                command,
                TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            orderRepository.AddCallCount.Should().Be(1);
            orderRepository.AddedOrder.Should().BeSameAs(result.Value);

            unitOfWork.SaveChangesCallCount.Should().Be(1);

            product.Stock.Should().Be(7);
        }

        [Fact]
        public async Task Should_return_conflict_when_stock_is_modified_concurrently()
        {
            // Arrange
            var customer = new Customer("Leonardo", "leonardo@email.com");
            var product = new Product("Notebook", 3500m, 10);

            var customerRepository = new CustomerRepositoryStub(customer);
            var productRepository = new ProductRepositoryStub(product);
            var orderRepository = new OrderRepositoryStub();
            var unitOfWork = new ConcurrencyUnitOfWorkStub();

            var handler = CreateHandler(customerRepository, productRepository, orderRepository, unitOfWork);

            var command = new CreateOrderCommand(customer.Id, product.Id, 3);

            // Act
            var result = await handler.Handle(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Product stock was modified by another operation. Please try again.");

            result.ErrorType.Should().Be(ResultErrorType.Conflict);
        }

        private static CreateOrderHandler CreateHandler(
            ICustomerRepository? customerRepository = null,
            IProductRepository? productRepository = null,
            IOrderRepository? orderRepository = null,
            IUnitOfWork? unitOfWork = null
        )
        {
            customerRepository ??= new CustomerRepositoryStub();
            productRepository ??= new ProductRepositoryStub();
            orderRepository ??= new OrderRepositoryStub();
            unitOfWork ??= new UnitOfWorkStub();

            return new CreateOrderHandler(customerRepository, productRepository, orderRepository, unitOfWork);
        }

        private sealed class ConcurrencyUnitOfWorkStub : IUnitOfWork
        {
            public Task<int> SaveChangesAsync(
                CancellationToken cancellationToken = default)
            {
                return Task.FromException<int>(
                    new ConcurrencyException("Product stock was modified by another operation. Please try again."));
            }
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

        private sealed class OrderRepositoryStub : IOrderRepository
        {
            public Order? AddedOrder { get; private set; }

            public int AddCallCount { get; private set; }

            public void Add(Order order)
            {
                AddedOrder = order;
                AddCallCount++;
            }
        }

        private sealed class UnitOfWorkStub : IUnitOfWork
        {
            public int SaveChangesCallCount { get; private set; }

            public Task<int> SaveChangesAsync(
                CancellationToken cancellationToken = default)
            {
                SaveChangesCallCount++;

                return Task.FromResult(1);
            }
        }
    }
}
