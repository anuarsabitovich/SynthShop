using FluentValidation;
using SynthShop.DTO;

namespace SynthShop.Validations
{
    public class AddBasketItemValidator : AbstractValidator<AddBasketItemDTO>
    {
        public AddBasketItemValidator()
        {
            RuleFor(dto => dto.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required.");

            RuleFor(dto => dto.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.");
        }
    }
}