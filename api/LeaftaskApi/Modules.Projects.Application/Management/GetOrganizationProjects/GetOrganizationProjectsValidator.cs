using BuildingBlocks.Application.Queries;
using BuildingBlocks.Application.Validation;
using BuildingBlocks.Domain.Result;
using FluentValidation;
using Modules.Projects.Application.Management.GetMyProjects;

namespace Modules.Projects.Application.Management.GetOrganizationProjects;

public sealed class GetOrganizationProjectsValidator : PaginatedQueryValidator<GetOrganizationProjectsQuery,
    Result<PaginatedResult<SimpleProjectDto>>>
{
    private static readonly HashSet<string> AllowedSortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "id",
        "name",
        "abbreviation",
        "createdAt"
    };

    public GetOrganizationProjectsValidator() =>
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
