using FluentValidation;
using SynthShop.DTO;

namespace SynthShop.Validations
{
    public class OrderValidator : AbstractValidator<CreateOrderDTO>
    {
        public OrderValidator() 
        {
            RuleFor(dto => dto.BasketId)
                .NotEmpty()
                .WithMessage("BasketId is required.");

        }
    }
}
