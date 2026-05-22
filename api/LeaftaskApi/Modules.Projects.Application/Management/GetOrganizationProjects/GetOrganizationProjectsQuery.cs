using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Management.GetMyProjects;

namespace Modules.Projects.Application.Management.GetOrganizationProjects;

public sealed class GetOrganizationProjectsQuery : IPaginatedQuery<Result<PaginatedResult<SimpleProjectDto>>>
{
    public Guid OrganizationId { get; init; }
    public int Limit { get; init; } = 10;
    public string? Cursor { get; init; }
    public IReadOnlyCollection<string> Sort { get; init; } = [];
}
