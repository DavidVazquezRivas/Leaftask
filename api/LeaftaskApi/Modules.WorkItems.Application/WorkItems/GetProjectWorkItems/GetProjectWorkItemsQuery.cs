using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.WorkItems.Application.WorkItems.GetProjectWorkItems;

[RequireProjectPermission("Access Project")]
public sealed class GetProjectWorkItemsQuery
    : IPaginatedQuery<Result<PaginatedResult<WorkItemListDto>>>, IProjectPermissionRequest
{
    public required Guid ProjectId { get; init; }
    public int Limit { get; init; } = 10;
    public string? Cursor { get; init; }
    public IReadOnlyCollection<string> Sort { get; init; } = [];
}
