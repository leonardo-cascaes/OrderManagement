using FluentAssertions;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Tests.Entities.Products
{
    public class ProductCreationTests
    {
        [Fact]
        public void Should_create_product()
        {
            // Assert
            var name = "Notebook";
            var price = 3500m;
            var stock = 10;

            // Act
            var product = new Product(name, price, stock);

            // Arrange
            product.Name.Should().Be(name);
            product.Price.Should().Be(price);
            product.Stock.Should().Be(stock);
        }

        [Fact]
        public void Should_not_create_product_without_name()
        {
            // Arrange
            var price = 3500m;
            var stock = 10;

            // Act
            Action act = () => new Product(string.Empty, price, stock);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_not_create_product_with_whitespace_name()
        {
            // Arrange
            var price = 3500m;
            var stock = 10;

            // Act
            Action act = () => new Product("   ", price, stock);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_not_create_product_with_zero_price()
        {
            // Arrange
            var name = "Notebook";
            var stock = 10;

            // Act
            Action act = () => new Product(name, 0m, stock);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_not_create_product_with_negative_price()
        {
            // Arrange
            var name = "Notebook";
            var stock = 10;

            // Act
            Action act = () => new Product(name, -1m, stock);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_not_create_product_with_negative_stock()
        {
            // Arrange
            var name = "Notebook";
            var price = 3500m;

            // Act
            Action act = () => new Product(name, price, -1);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_create_product_with_zero_stock()
        {
            // Arrange
            var name = "Notebook";
            var price = 3500m;

            // Act
            var product = new Product(name, price, 0);

            // Assert
            product.Stock.Should().Be(0);
        }
    }
}
