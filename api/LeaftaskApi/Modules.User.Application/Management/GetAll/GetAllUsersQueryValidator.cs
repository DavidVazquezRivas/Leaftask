using BuildingBlocks.Application.Validation;
using BuildingBlocks.Domain.Result;
using FluentValidation;

namespace Modules.Users.Application.Management.GetAll;

public sealed class GetAllUsersQueryValidator
    : PaginatedQueryValidator<GetAllUsersQuery, Result<IReadOnlyCollection<SimpleUserDto>>>
{
    public GetAllUsersQueryValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(50)
            .WithMessage("The search cannot exceed 50 characters.");
    }
}
