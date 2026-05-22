using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;

namespace Modules.WorkItems.Application.WorkLogs.Delete;

[RequireProjectPermission("Access Project")]
public sealed record DeleteWorkLogCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Guid LogId) : ICommand<Result>, IProjectPermissionRequest;
