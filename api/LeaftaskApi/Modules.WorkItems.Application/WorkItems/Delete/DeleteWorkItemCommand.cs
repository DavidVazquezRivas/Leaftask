using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.WorkItems.Application.WorkItems.Delete;

[RequireProjectPermission("work-items.create")]
public sealed record DeleteWorkItemCommand(Guid ProjectId, Guid WorkItemId)
    : ICommand<Result>, IProjectPermissionRequest;
