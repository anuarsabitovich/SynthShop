using FluentValidation;
using SynthShop.DTO;

namespace SynthShop.Validations;

public class ProductValidator : AbstractValidator<AddProductDTO>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(100)
            .WithMessage("Product name must be at most 100 characters.");

        RuleFor(p => p.Description)
            .NotEmpty()
            .WithMessage("Product description is required.")
            .MaximumLength(1000)
            .WithMessage("Product description must be at most 1000 characters.");

        RuleFor(p => p.Price)
            .NotEmpty()
            .WithMessage("Product price is required.")
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.");

        RuleFor(p => p.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity must be non-negative.");

        RuleFor(p => p.CategoryID)
            .NotEmpty()
            .WithMessage("Category ID is required.");
    }
}