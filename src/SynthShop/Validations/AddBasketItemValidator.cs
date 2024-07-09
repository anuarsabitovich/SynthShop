using FluentValidation;
using SynthShop.DTO;

namespace SynthShop.Validations;

public class UpdateBasketItemValidator : AbstractValidator<UpdateBaskItemDTO>
{
    public UpdateBasketItemValidator()
    {
        RuleFor(dto => dto.BasketItemId)
            .NotEmpty()
            .WithMessage("BasketItemId is required.");

        RuleFor(dto => dto.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.");
    }
}