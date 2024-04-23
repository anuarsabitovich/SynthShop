using FluentValidation;
using SynthShop.Domain.Entities;

namespace SynthShop.Validations
{
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(o => o.CustomerID)
                .NotEmpty()
                .WithMessage("Customer ID is required.");
            
            RuleFor(o => o.OrderItems)
                .NotEmpty()
                .WithMessage("An order must contain at least one item.");
        }
    }
}
