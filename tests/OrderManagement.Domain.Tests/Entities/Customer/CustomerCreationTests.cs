using FluentAssertions;
using OrderManagement.Domain.Entities;

namespace OrderManagement.Domain.Tests.Entities
{
    public class CustomerCreationTests
    {
        [Fact]
        public void Should_create_customer()
        {
            // Arrange
            var name = "Leonardo";
            var email = "leonardo@email.com";

            // Act
            var customer = new Customer(name, email);

            // Assert
            customer.Name.Should().Be(name);
            customer.Email.Should().Be(email);
        }

        [Fact]
        public void Should_not_create_customer_without_name()
        {
            // Arrange
            var email = "leonardo@email.com";

            // Act
            Action act = () => new Customer(string.Empty, email);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_not_create_customer_without_email()
        {
            // Arrange
            var name = "Leonardo";

            // Act
            Action act = () => new Customer(name, string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_not_create_customer_with_invalid_email()
        {
            // Arrange
            var name = "Leonardo";
            var email = "email-invalido";

            // Act
            Action act = () => new Customer(name, email);

            // Assert
            act.Should().Throw<ArgumentException>();
        }
    }
}
