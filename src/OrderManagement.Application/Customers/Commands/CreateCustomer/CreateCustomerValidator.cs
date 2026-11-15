using FluentValidation;

namespace OrderManagement.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name cannot be empty.");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Email cannot be empty.")
            .Must(email => email.Contains('@') && email.Contains('.'))
            .WithMessage("Invalid email.");
    }
}