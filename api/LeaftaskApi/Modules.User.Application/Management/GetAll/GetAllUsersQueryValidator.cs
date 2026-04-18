using BuildingBlocks.Application.Queries;
using BuildingBlocks.Application.Validation;
using BuildingBlocks.Domain.Result;
using FluentValidation;

namespace Modules.Users.Application.Management.GetAll;

public sealed class GetAllUsersQueryValidator
    : PaginatedQueryValidator<GetAllUsersQuery, Result<PaginatedResult<SimpleUserDto>>>
{
    private static readonly HashSet<string> AllowedSortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "id",
        "firstName",
        "lastName",
        "email"
    };

    public GetAllUsersQueryValidator()
    {
        RuleFor(x => x.Search)
            .MaximumLength(50)
            .WithMessage("The search cannot exceed 50 characters.");

        RuleFor(x => x.Sort)
            .Must(BeValidSortFields)
            .WithMessage("The sort fields must be valid and unique.");
    }

    private static bool BeValidSortFields(IReadOnlyCollection<string> sort)
    {
        HashSet<string> fields = new(StringComparer.OrdinalIgnoreCase);

        foreach (string item in sort)
        {
            string[] parts = item.Split(':', 2);
            if (parts.Length != 2 || !AllowedSortFields.Contains(parts[0]))
            {
                return false;
            }

            if (!fields.Add(parts[0]))
            {
                return false;
            }
        }

        return true;
    }
}
