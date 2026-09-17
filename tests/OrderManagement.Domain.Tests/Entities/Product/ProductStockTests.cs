using FluentAssertions;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Tests.Entities
{
    public class ProductStockTests
    {
        [Fact]
        public void Should_increase_product_stock()
        {
            // Arrange
            var product = new Product("Notebook", 3500m, 10);

            // Act
            product.AddStock(5);

            // Assert
            product.Stock.Should().Be(15);
        }

        [Fact]
        public void Should_not_add_zero_stock()
        {
            // Arrange
            var product = new Product("Notebook", 3500m, 10);

            // Act
            Action act = () => product.AddStock(0);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_not_add_negative_stock()
        {
            // Arrange
            var product = new Product("Notebook", 3500m, 10);

            // Act
            Action act = () => product.AddStock(-5);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_decrease_product_stock()
        {
            // Arrange
            var product = new Product("Notebook", 3500m, 10);

            // Act
            product.RemoveStock(3);

            // Assert
            product.Stock.Should().Be(7);
        }

        [Fact]
        public void Should_not_remove_more_stock_than_available()
        {
            // Arrange
            var product = new Product("Notebook", 3500m, 10);

            // Act
            Action act = () => product.RemoveStock(11);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Should_not_remove_zero_stock()
        {
            // Arrange
            var product = new Product("Notebook", 3500m, 10);

            // Act
            Action act = () => product.RemoveStock(0);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_not_remove_negative_stock()
        {
            // Arrange
            var product = new Product("Notebook", 3500m, 10);

            // Act
            Action act = () => product.RemoveStock(-5);

            // Assert
            act.Should().Throw<ArgumentException>();
        }
    }
}
