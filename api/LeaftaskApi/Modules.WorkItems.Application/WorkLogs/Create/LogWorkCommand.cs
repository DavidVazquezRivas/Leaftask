using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.WorkItems.Application.WorkLogs.Create;

[RequireProjectPermission("work-items.edit-own-progress")]
public sealed record LogWorkCommand(
    Guid ProjectId,
    Guid WorkItemId,
    decimal Dedication,
    DateOnly Date,
    string Description) : ICommand<Result<WorkLogDto>>, IProjectPermissionRequest;
