using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;

namespace Modules.WorkItems.Application.WorkLogs.List;

[RequireProjectPermission("Access Project")]
public sealed record GetWorkLogsQuery(
    Guid ProjectId,
    Guid WorkItemId,
    int Limit,
    string? Cursor,
    IReadOnlyCollection<string> Sort) : IQuery<Result<PaginatedResult<WorkLogDto>>>, IProjectPermissionRequest;
