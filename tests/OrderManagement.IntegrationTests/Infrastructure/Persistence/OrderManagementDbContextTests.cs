using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using OrderManagement.Domain.Enums;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.IntegrationTests.Infrastructure.Persistence
{
    public class OrderManagementDbContextTests
    {
        [Fact]
        public void Should_include_domain_entities_in_model()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            using var context = new OrderManagementDbContext(options);

            // Act
            var entityTypes = context.Model.GetEntityTypes().Select(entityType => entityType.ClrType);

            // Assert
            entityTypes.Should().Contain(typeof(Customer));
            entityTypes.Should().Contain(typeof(Product));
            entityTypes.Should().Contain(typeof(Order));
            entityTypes.Should().Contain(typeof(OrderItem));
        }

        [Fact]
        public void Should_map_customer_to_customers_table()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            using var context = new OrderManagementDbContext(options);

            // Act
            var entityType = context.Model.FindEntityType(typeof(Customer));

            // Assert
            entityType.Should().NotBeNull();
            entityType.GetTableName().Should().Be("customers");
        }

        [Fact]
        public void Should_map_product_to_products_table()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            using var context = new OrderManagementDbContext(options);

            // Act
            var entityType = context.Model.FindEntityType(typeof(Product));

            // Assert
            entityType.Should().NotBeNull();
            entityType.GetTableName().Should().Be("products");
        }

        [Fact]
        public void Should_configure_product_price_precision()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            using var context = new OrderManagementDbContext(options);

            // Act
            var property = context.Model.FindEntityType(typeof(Product))!.FindProperty(nameof(Product.Price));

            // Assert
            property.Should().NotBeNull();
            property!.GetPrecision().Should().Be(18);
            property.GetScale().Should().Be(2);
        }

        [Fact]
        public void Should_map_order_to_orders_table()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            using var context = new OrderManagementDbContext(options);

            // Act
            var entityType = context.Model.FindEntityType(typeof(Order));

            // Assert
            entityType.Should().NotBeNull();
            entityType.GetTableName().Should().Be("orders");
        }

        [Fact]
        public void Should_configure_required_customer_relationship_for_order()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            using var context = new OrderManagementDbContext(options);

            // Act
            var orderEntity = context.Model.FindEntityType(typeof(Order));

            var foreignKeys = orderEntity!.GetForeignKeys()
                .Where(fk => fk.PrincipalEntityType.ClrType == typeof(Customer))
                .ToList();

            // Assert
            foreignKeys.Should().ContainSingle();

            var foreignKey = foreignKeys.Single();

            foreignKey.Properties.Should().ContainSingle();

            foreignKey.Properties.Single().Name.Should().Be(nameof(Order.CustomerId));

            foreignKey.IsRequired.Should().BeTrue();
        }

        [Fact]
        public async Task Should_not_delete_customer_when_orders_exist()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;

            await using var connection = new SqliteConnection("Data Source=:memory:");

            await connection.OpenAsync(cancellationToken);

            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite(connection)
                .Options;

            var customer = new Customer("Leonardo", "leonardo@email.com");

            await using (var context = new OrderManagementDbContext(options))
            {
                await context.Database.EnsureCreatedAsync(cancellationToken);

                var order = new Order(customer.Id);

                context.Customers.Add(customer);
                context.Orders.Add(order);

                await context.SaveChangesAsync(cancellationToken);
            }

            // Act
            await using var deleteContext =
                new OrderManagementDbContext(options);

            var customerToDelete = await deleteContext.Customers.SingleAsync(x => x.Id == customer.Id, cancellationToken: cancellationToken);

            deleteContext.Customers.Remove(customerToDelete);

            Func<Task> act = () => deleteContext.SaveChangesAsync(cancellationToken);

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public void Should_configure_order_item_relationship_with_order()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            using var context = new OrderManagementDbContext(options);

            // Act
            var orderItemEntity = context.Model.FindEntityType(typeof(OrderItem));

            var foreignKey = orderItemEntity!.GetForeignKeys()
                .Single(fk => fk.PrincipalEntityType.ClrType == typeof(Order));

            // Assert
            foreignKey.IsRequired.Should().BeTrue();

            foreignKey.DeleteBehavior.Should().Be(DeleteBehavior.Cascade);

            foreignKey.Properties.Should().ContainSingle();

            var orderIdProperty = foreignKey.Properties.Single();

            orderIdProperty.IsShadowProperty().Should().BeTrue();

            orderIdProperty.GetColumnName().Should().Be("order_id");
        }

        [Fact]
        public void Should_configure_required_product_relationship_for_order_item()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            using var context = new OrderManagementDbContext(options);

            // Act
            var orderItemEntity = context.Model.FindEntityType(typeof(OrderItem));

            var foreignKey = orderItemEntity!.GetForeignKeys()
                .Single(fk => fk.PrincipalEntityType.ClrType == typeof(Product));

            // Assert
            foreignKey.IsRequired.Should().BeTrue();

            foreignKey.DeleteBehavior.Should().Be(DeleteBehavior.Restrict);

            foreignKey.Properties.Should().ContainSingle();

            var productIdProperty = foreignKey.Properties.Single();

            productIdProperty.Name.Should().Be(nameof(OrderItem.ProductId));
            productIdProperty.IsShadowProperty().Should().BeFalse();
        }

        [Fact]
        public async Task Should_not_delete_product_when_order_items_exist()
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

            var order = new Order(customer.Id);

            order.AddItem(product.Id, 1, product.Price);

            await using (var context = new OrderManagementDbContext(options))
            {
                await context.Database.EnsureCreatedAsync(cancellationToken);

                context.Customers.Add(customer);
                context.Products.Add(product);
                context.Orders.Add(order);

                await context.SaveChangesAsync(cancellationToken);
            }

            // Act
            await using var deleteContext = new OrderManagementDbContext(options);

            var productToDelete = await deleteContext.Products
                .SingleAsync(x => x.Id == product.Id, cancellationToken);

            deleteContext.Products.Remove(productToDelete);

            Func<Task> act = () => deleteContext.SaveChangesAsync(cancellationToken);

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task Should_delete_order_items_when_order_is_deleted()
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

            var order = new Order(customer.Id);

            order.AddItem(product.Id, 2, product.Price);

            await using (var context = new OrderManagementDbContext(options))
            {
                await context.Database.EnsureCreatedAsync(cancellationToken);

                context.Customers.Add(customer);
                context.Products.Add(product);
                context.Orders.Add(order);

                await context.SaveChangesAsync(cancellationToken);
            }

            // Act
            await using (var deleteContext = new OrderManagementDbContext(options))
            {
                var orderToDelete = await deleteContext.Orders
                    .SingleAsync(x => x.Id == order.Id, cancellationToken);

                deleteContext.Orders.Remove(orderToDelete);

                await deleteContext.SaveChangesAsync(cancellationToken);
            }

            // Assert
            await using var verifyContext = new OrderManagementDbContext(options);

            var remainingOrders = await verifyContext.Orders.CountAsync(cancellationToken);

            var remainingItems = await verifyContext.OrderItems.CountAsync(cancellationToken);

            remainingOrders.Should().Be(0);
            remainingItems.Should().Be(0);
        }

        [Fact]
        public async Task Should_persist_and_load_order_with_order_items()
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

            var order = new Order(customer.Id);

            order.AddItem(product.Id, 2, product.Price);

            // Act
            await using (var context = new OrderManagementDbContext(options))
            {
                await context.Database.EnsureCreatedAsync(cancellationToken);

                context.Customers.Add(customer);
                context.Products.Add(product);
                context.Orders.Add(order);

                await context.SaveChangesAsync(cancellationToken);
            }

            await using var verifyContext = new OrderManagementDbContext(options);

            var persistedOrder = await verifyContext.Orders
                .Include(x => x.OrderItems)
                .SingleAsync(x => x.Id == order.Id, cancellationToken);

            // Assert
            persistedOrder.Should().NotBeNull();

            persistedOrder.Id.Should().Be(order.Id);
            persistedOrder.CustomerId.Should().Be(customer.Id);
            persistedOrder.Status.Should().Be(OrderStatus.Pending);

            persistedOrder.OrderItems.Should().ContainSingle();

            var item = persistedOrder.OrderItems.Single();

            item.ProductId.Should().Be(product.Id);
            item.Quantity.Should().Be(2);
            item.UnitPrice.Should().Be(3500m);

            persistedOrder.Total.Should().Be(7000m);
        }

        [Fact]
        public void Should_configure_order_item_unit_price_precision()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            using var context = new OrderManagementDbContext(options);

            // Act
            var property = context.Model.FindEntityType(typeof(OrderItem))!.FindProperty(nameof(OrderItem.UnitPrice));

            // Assert
            property.Should().NotBeNull();
            property!.GetPrecision().Should().Be(18);
            property.GetScale().Should().Be(2);
        }

        [Fact]
        public async Task Should_persist_entity_audit_timestamps()
        {
            // Arrange
            var cancellationToken = TestContext.Current.CancellationToken;

            await using var connection = new SqliteConnection("Data Source=:memory:");

            await connection.OpenAsync(cancellationToken);

            var options = new DbContextOptionsBuilder<OrderManagementDbContext>()
                .UseSqlite(connection)
                .Options;

            var customer = new Customer("Leonardo", "leonardo@email.com");

            var createdAt = customer.CreatedAt;

            // Act
            await using (var context = new OrderManagementDbContext(options))
            {
                await context.Database.EnsureCreatedAsync(cancellationToken);

                context.Customers.Add(customer);

                await context.SaveChangesAsync(cancellationToken);
            }

            await using var verifyContext =
                new OrderManagementDbContext(options);

            var persistedCustomer = await verifyContext.Customers
                .SingleAsync(x => x.Id == customer.Id, cancellationToken);

            // Assert
            persistedCustomer.CreatedAt.Should().Be(createdAt);
            persistedCustomer.UpdatedAt.Should().BeNull();
        }
    }
}
