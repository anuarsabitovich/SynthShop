using FluentValidation;
using SynthShop.DTO;

namespace SynthShop.Validations
{
    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
    {
        public RegistrationRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.FirstName)); 

            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.LastName)); 

            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.Address)); 
        }
    }
}