using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Application.WorkLogs.List;

public sealed record GetWorkLogsQuery(
    Guid ProjectId,
    Guid WorkItemId,
    int Limit,
    string? Cursor,
    IReadOnlyCollection<string> Sort) : IQuery<Result<PaginatedResult<WorkLogDto>>>;
