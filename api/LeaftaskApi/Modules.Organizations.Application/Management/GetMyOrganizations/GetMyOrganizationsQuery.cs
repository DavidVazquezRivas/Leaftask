using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Management.GetMyOrganizations;

public sealed class GetMyOrganizationsQuery : IPaginatedQuery<Result<PaginatedResult<SimpleOrganizationDto>>>
{
    public int Limit { get; init; } = 10;
    public string? Cursor { get; init; }
    public IReadOnlyCollection<string> Sort { get; init; } = [];
}
