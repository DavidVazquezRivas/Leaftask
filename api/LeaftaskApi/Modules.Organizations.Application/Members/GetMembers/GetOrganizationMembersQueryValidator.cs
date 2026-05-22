using BuildingBlocks.Application.Queries;
using BuildingBlocks.Application.Validation;
using BuildingBlocks.Domain.Result;
using FluentValidation;

namespace Modules.Organizations.Application.Members.GetMembers;

public sealed class GetOrganizationMembersQueryValidator
    : PaginatedQueryValidator<GetOrganizationMembersQuery, Result<PaginatedResult<OrganizationMemberDto>>>
{
    private static readonly HashSet<string> AllowedSortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "id",
        "name",
        "email",
        "role"
    };

    public GetOrganizationMembersQueryValidator() =>
        RuleFor(x => x.Sort)
            .Must(BeValidSortFields)
            .WithMessage("The sort fields must be valid and unique.");

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
