using FluentValidation;
using SynthShop.DTO;

namespace SynthShop.Validations;

public class CustomerValidator : AbstractValidator<AddCustomerDTO>
{
    public CustomerValidator()
    {
        RuleFor(c => c.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.")
            .MaximumLength(50)
            .WithMessage("First name must be at most 50 characters.");

        RuleFor(c => c.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.")
            .MaximumLength(50)
            .WithMessage("Last name must be at most 50 characters.");

        RuleFor(c => c.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(c => c.Address)
            .MaximumLength(200)
            .WithMessage("Address must be at most 200 characters.");
    }
}