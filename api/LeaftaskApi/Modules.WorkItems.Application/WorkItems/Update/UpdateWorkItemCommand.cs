using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;

namespace Modules.WorkItems.Application.WorkItems.Update;

[RequireProjectPermission("Access Project")]
public sealed record UpdateWorkItemCommand(
    Guid ProjectId,
    Guid WorkItemId,
    string? Title,
    string? Description,
    Guid? StatusId,
    Guid? TypeId,
    Guid? AssigneeId,
    bool UpdateAssignee,
    int? Progress,
    decimal? Estimation,
    DateTime? LimitDate,
    Guid? ParentId,
    bool UpdateParent,
    IReadOnlyDictionary<Guid, string> CustomFields)
    : ICommand<Result<WorkItemDetailDto>>, IProjectPermissionRequest;
