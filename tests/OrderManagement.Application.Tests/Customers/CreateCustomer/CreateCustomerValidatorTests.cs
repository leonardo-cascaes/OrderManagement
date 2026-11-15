using FluentValidation.TestHelper;
using OrderManagement.Application.Customers.Commands.CreateCustomer;

namespace OrderManagement.Application.Tests.Customers.CreateCustomer;

public class CreateCustomerValidatorTests
{
    private readonly CreateCustomerValidator _validator = new();

    [Fact]
    public void Should_have_error_when_name_is_empty()
    {
        // Arrange
        var command = new CreateCustomerCommand("", "leonardo@email.com");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_name_is_whitespace()
    {
        // Arrange
        var command = new CreateCustomerCommand("   ", "leonardo@email.com");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_email_is_empty()
    {
        // Arrange
        var command = new CreateCustomerCommand("Leonardo", "");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_have_error_when_email_is_invalid()
    {
        // Arrange
        var command = new CreateCustomerCommand("Leonardo", "invalid-email");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_not_have_errors_when_command_is_valid()
    {
        // Arrange
        var command = new CreateCustomerCommand("Leonardo", "leonardo@email.com");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}