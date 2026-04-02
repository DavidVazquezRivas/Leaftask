using BuildingBlocks.Application.Queries;
using FluentValidation;

namespace BuildingBlocks.Application.Validation;

public abstract class PaginatedQueryValidator<TQuery, TResponse> : AbstractValidator<TQuery>
    where TQuery : IPaginatedQuery<TResponse>
{
    protected PaginatedQueryValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 100)
            .WithMessage("Limit must be between 1 and 100.");

        RuleFor(x => x.Cursor)
            .Must(cursor => Guid.TryParse(cursor, out _))
            .When(x => !string.IsNullOrEmpty(x.Cursor))
            .WithMessage("Cursor must be a valid Guid.");

        RuleForEach(x => x.Sort)
            .Matches(@"^[a-zA-Z0-9_]+:(asc|desc)$")
            .WithMessage("The sort format must be 'field:asc' or 'field:desc'.");
    }
}
