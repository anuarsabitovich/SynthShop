using FluentValidation;
using SynthShop.Domain.Entities;
using SynthShop.DTO;

namespace SynthShop.Validations
{
    public class OrderValidator : AbstractValidator<CreateOrderDTO>
    {
        public OrderValidator() 
        {
            RuleFor(dto => dto.CustomerId)
                .NotEmpty()
                .WithMessage("CustomerId is required.");

            RuleFor(dto => dto.BasketId)
                .NotEmpty()
                .WithMessage("BasketId is required.");

        }
    }
}
