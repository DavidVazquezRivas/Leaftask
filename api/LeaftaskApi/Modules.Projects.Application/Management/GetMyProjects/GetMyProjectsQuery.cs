using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Management.GetMyProjects;

public sealed class GetMyProjectsQuery : IPaginatedQuery<Result<PaginatedResult<SimpleProjectDto>>>
{
    public int Limit { get; init; } = 10;
    public string? Cursor { get; init; }
    public IReadOnlyCollection<string> Sort { get; init; } = [];
}
