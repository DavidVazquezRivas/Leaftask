using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;

namespace Modules.WorkItems.Application.WorkItems.Delete;

[RequireProjectPermission("Access Project")]
public sealed record DeleteWorkItemCommand(Guid ProjectId, Guid WorkItemId)
    : ICommand<Result>, IProjectPermissionRequest;
