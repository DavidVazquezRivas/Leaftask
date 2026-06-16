using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.WorkItems.Application.WorkLogs.Update;

[RequireProjectPermission("work-items.edit-own-progress")]
public sealed record UpdateWorkLogCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Guid LogId,
    decimal? Dedication,
    DateOnly? Date,
    string? Description) : ICommand<Result<WorkLogDto>>, IProjectPermissionRequest;
