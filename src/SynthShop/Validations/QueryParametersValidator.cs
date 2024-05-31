using FluentValidation;
using SynthShop.Queries;

namespace SynthShop.Validations
{
    public class QueryParametersValidator : AbstractValidator<SearchQueryParameters>
    {
        public QueryParametersValidator()
        {

            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1");

            RuleFor(x => x.PageSize)
                .Must(x => x is null or >= 0 and <= 1000)
                .WithMessage("Page size must be between 1 and 1000");
        }

    }
}
