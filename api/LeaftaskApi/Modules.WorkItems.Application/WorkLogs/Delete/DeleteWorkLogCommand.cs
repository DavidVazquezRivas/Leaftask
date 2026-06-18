using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.WorkItems.Application.WorkLogs.Delete;

[RequireProjectPermission("work-items.edit-own-progress")]
public sealed record DeleteWorkLogCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Guid LogId) : ICommand<Result>, IProjectPermissionRequest;
