using BuildingBlocks.Application.Queries;

namespace Modules.WorkItems.Application.WorkLogs.List;

public interface IGetWorkLogsQueryService
{
    Task<PaginatedResult<WorkLogDto>> GetWorkLogsAsync(
        Guid projectId,
        Guid workItemId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default);
}
