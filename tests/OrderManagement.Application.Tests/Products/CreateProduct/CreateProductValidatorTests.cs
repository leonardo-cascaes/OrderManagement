using FluentAssertions;
using FluentValidation.TestHelper;
using OrderManagement.Application.Products.Commands.CreateProduct;

namespace OrderManagement.Application.Tests.Products.CreateProduct;

public class CreateProductValidatorTests
{
    private readonly CreateProductValidator _validator = new();

    [Fact]
    public void Should_have_error_when_name_is_empty()
    {
        // Arrange
        var command = new CreateProductCommand("", 3500m, 10);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_name_is_whitespace()
    {
        // Arrange
        var command = new CreateProductCommand("   ", 3500m, 10);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_have_error_when_price_is_zero()
    {
        // Arrange
        var command = new CreateProductCommand("Notebook", 0m, 10);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Should_have_error_when_price_is_negative()
    {
        // Arrange
        var command = new CreateProductCommand("Notebook", -100m, 10);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Should_have_error_when_stock_is_negative()
    {
        // Arrange
        var command = new CreateProductCommand("Notebook", 3500m, -1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Stock);
    }

    [Fact]
    public void Should_not_have_errors_when_command_is_valid()
    {
        // Arrange
        var command = new CreateProductCommand("Notebook", 3500m, 10);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_allow_zero_stock()
    {
        // Arrange
        var command = new CreateProductCommand("Notebook", 3500m, 0);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Stock);
    }
}