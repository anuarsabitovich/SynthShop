using FluentValidation;
using SynthShop.DTO;

namespace SynthShop.Validations
{
    public class CategoryValidator : AbstractValidator<AddCategoryDTO>
    {
        public CategoryValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("Category name is required.")
                .MaximumLength(100)
                .WithMessage("Category name must be at most 100 characters");

            RuleFor(c => c.Description)
                .NotEmpty()
                .WithMessage("Category description")
                .MaximumLength(1000)
                .WithMessage("Category description cannot be more than 1000 characters");
            
            
        }
    }
}
