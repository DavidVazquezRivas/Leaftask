using BuildingBlocks.Application.Queries;

namespace Modules.WorkItems.Application.WorkItems.GetProjectWorkItems;

public interface IGetProjectWorkItemsQueryService
{
    Task<PaginatedResult<WorkItemListDto>> GetProjectWorkItemsAsync(
        Guid projectId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default);
}
