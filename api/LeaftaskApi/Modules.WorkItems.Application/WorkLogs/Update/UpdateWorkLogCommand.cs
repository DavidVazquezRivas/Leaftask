using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;

namespace Modules.WorkItems.Application.WorkLogs.Update;

[RequireProjectPermission("Access Project")]
public sealed record UpdateWorkLogCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Guid LogId,
    decimal? Dedication,
    DateOnly? Date,
    string? Description) : ICommand<Result<WorkLogDto>>, IProjectPermissionRequest;
