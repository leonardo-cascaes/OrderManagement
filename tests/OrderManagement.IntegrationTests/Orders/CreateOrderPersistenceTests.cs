using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Common;
using OrderManagement.Application.DependencyInjection;
using OrderManagement.Application.Interfaces.Repositories;
using OrderManagement.Application.Orders.Commands.CreateOrder;
using OrderManagement.Application.Persistence;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Infrastructure.Persistence;
using OrderManagement.Infrastructure.Persistence.Repositories;

namespace OrderManagement.IntegrationTests.Orders;

public class CreateOrderPersistenceTests
{
    [Fact]
    public async Task Should_persist_order_and_decrease_product_stock_when_order_is_created()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var connection = new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync(cancellationToken);

        var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseSqlite(connection)
            .Options;

        var customer = new Customer("Leonardo", "leonardo@email.com");
        var product = new Product("Notebook", 3500m, 10);

        await using (var arrangeContext = new OrderManagementDbContext(options))
        {
            await arrangeContext.Database.EnsureCreatedAsync(cancellationToken);

            arrangeContext.Customers.Add(customer);
            arrangeContext.Products.Add(product);

            await arrangeContext.SaveChangesAsync(cancellationToken);
        }

        // Act
        await using (var actContext = new OrderManagementDbContext(options))
        {
            var handler = new CreateOrderHandler(
                new CustomerRepository(actContext),
                new ProductRepository(actContext),
                new OrderRepository(actContext),
                actContext);

            var command = new CreateOrderCommand(customer.Id, product.Id, 3);

            var result = await handler.Handle(command, cancellationToken);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.CustomerId.Should().Be(customer.Id);
            result.Value.Status.Should().Be(OrderStatus.Pending);
        }

        // Assert
        await using var assertContext = new OrderManagementDbContext(options);

        var persistedProduct = await assertContext.Products
            .SingleAsync(x => x.Id == product.Id, cancellationToken);

        var persistedOrder = await assertContext.Orders.Include(x => x.OrderItems)
            .SingleAsync(cancellationToken);

        persistedProduct.Stock.Should().Be(7);

        persistedOrder.CustomerId.Should().Be(customer.Id);
        persistedOrder.Status.Should().Be(OrderStatus.Pending);
        persistedOrder.Total.Should().Be(10500m);

        persistedOrder.OrderItems.Should().ContainSingle();

        var item = persistedOrder.OrderItems.Single();

        item.ProductId.Should().Be(product.Id);
        item.Quantity.Should().Be(3);
        item.UnitPrice.Should().Be(3500m);
    }

    [Fact]
    public async Task Should_not_persist_order_or_decrease_product_stock_when_stock_is_insufficient()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var connection = new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync(cancellationToken);

        var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseSqlite(connection)
            .Options;

        var customer = new Customer("Leonardo", "leonardo@email.com");
        var product = new Product("Notebook", 3500m, 2);

        await using (var arrangeContext = new OrderManagementDbContext(options))
        {
            await arrangeContext.Database.EnsureCreatedAsync(cancellationToken);

            arrangeContext.Customers.Add(customer);
            arrangeContext.Products.Add(product);

            await arrangeContext.SaveChangesAsync(cancellationToken);
        }

        // Act
        await using (var actContext = new OrderManagementDbContext(options))
        {
            var handler = new CreateOrderHandler(
                new CustomerRepository(actContext),
                new ProductRepository(actContext),
                new OrderRepository(actContext),
                actContext);

            var command = new CreateOrderCommand(customer.Id, product.Id, 3);

            var result = await handler.Handle(command, cancellationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Insufficient stock.");
            result.ErrorType.Should().Be(ResultErrorType.Conflict);
        }

        // Assert persistence
        await using var assertContext = new OrderManagementDbContext(options);

        var persistedProduct = await assertContext.Products
            .SingleAsync(x => x.Id == product.Id, cancellationToken);

        var orderCount = await assertContext.Orders.CountAsync(cancellationToken);

        persistedProduct.Stock.Should().Be(2);
        orderCount.Should().Be(0);
    }

    [Fact]
    public async Task Should_not_persist_order_or_decrease_stock_when_quantity_is_invalid()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var connection = new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync(cancellationToken);

        var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseSqlite(connection)
            .Options;

        var customer = new Customer("Leonardo", "leonardo@email.com");
        var product = new Product("Notebook", 3500m, 10);

        await using (var arrangeContext = new OrderManagementDbContext(options))
        {
            await arrangeContext.Database.EnsureCreatedAsync(cancellationToken);

            arrangeContext.Customers.Add(customer);
            arrangeContext.Products.Add(product);

            await arrangeContext.SaveChangesAsync(cancellationToken);
        }

        var services = new ServiceCollection();

        services.AddLogging();
        services.AddApplication();

        services.AddDbContext<OrderManagementDbContext>(options =>
            options.UseSqlite(connection));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<OrderManagementDbContext>());

        using var provider = services.BuildServiceProvider();

        // Act
        using (var scope = provider.CreateScope())
        {
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var command = new CreateOrderCommand(customer.Id, product.Id, 0);

            Func<Task> act = () => sender.Send(command, cancellationToken);

            await act.Should().ThrowAsync<ValidationException>();
        }

        // Assert
        await using var assertContext = new OrderManagementDbContext(options);

        var persistedProduct = await assertContext.Products
            .SingleAsync(x => x.Id == product.Id, cancellationToken);

        var orderCount = await assertContext.Orders.CountAsync(cancellationToken);

        persistedProduct.Stock.Should().Be(10);
        orderCount.Should().Be(0);
    }

    [Fact]
    public async Task Should_return_conflict_and_not_persist_second_order_when_stock_was_modified_concurrently()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        await using var connection = new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync(cancellationToken);

        var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
            .UseSqlite(connection)
            .Options;

        var customer = new Customer("Leonardo", "leonardo@email.com");
        var product = new Product("Notebook", 3500m, 10);

        await using (var arrangeContext = new OrderManagementDbContext(options))
        {
            await arrangeContext.Database.EnsureCreatedAsync(cancellationToken);

            arrangeContext.Customers.Add(customer);
            arrangeContext.Products.Add(product);

            await arrangeContext.SaveChangesAsync(cancellationToken);
        }

        await using var firstContext = new OrderManagementDbContext(options);
        await using var secondContext = new OrderManagementDbContext(options);

        // Ambos carregam o produto enquanto o estoque ainda é 10.
        var firstProduct = await firstContext.Products
            .SingleAsync(x => x.Id == product.Id, cancellationToken);

        var secondProduct = await secondContext.Products
            .SingleAsync(x => x.Id == product.Id, cancellationToken);

        firstProduct.Stock.Should().Be(10);
        secondProduct.Stock.Should().Be(10);

        var firstHandler = new CreateOrderHandler(
            new CustomerRepository(firstContext),
            new ProductRepository(firstContext),
            new OrderRepository(firstContext),
            firstContext);

        var secondHandler = new CreateOrderHandler(
            new CustomerRepository(secondContext),
            new ProductRepository(secondContext),
            new OrderRepository(secondContext),
            secondContext);

        // Act
        var firstResult = await firstHandler.Handle(new CreateOrderCommand(customer.Id, product.Id, 6), cancellationToken);

        var secondResult = await secondHandler.Handle(new CreateOrderCommand(customer.Id, product.Id, 5), cancellationToken);

        // Assert
        firstResult.IsSuccess.Should().BeTrue();

        secondResult.IsSuccess.Should().BeFalse();
        secondResult.ErrorType.Should().Be(ResultErrorType.Conflict);

        secondResult.Error.Should().Be(
            "Product stock was modified by another operation. Please try again.");

        // Verificar o estado persistido
        await using var assertContext = new OrderManagementDbContext(options);

        var persistedProduct = await assertContext.Products
            .SingleAsync(x => x.Id == product.Id, cancellationToken);

        var persistedOrders = await assertContext.Orders
            .Include(x => x.OrderItems)
            .ToListAsync(cancellationToken);

        persistedProduct.Stock.Should().Be(4);

        persistedOrders.Should().ContainSingle();

        var persistedOrder = persistedOrders.Single();

        persistedOrder.OrderItems.Should().ContainSingle();
        persistedOrder.OrderItems.Single().Quantity.Should().Be(6);
    }
}