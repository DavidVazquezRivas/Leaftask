using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;
using Modules.WorkItems.Application.WorkItems;

namespace Modules.WorkItems.Application.WorkItems.Create;

[RequireProjectPermission("Access Project")]
public sealed record CreateWorkItemCommand(
    Guid ProjectId,
    string Title,
    string Description,
    decimal Estimation,
    Guid TypeId,
    Guid StatusId,
    Guid? AssigneeId,
    Guid ParentId,
    IReadOnlyDictionary<Guid, string> CustomFields) : ICommand<Result<WorkItemDetailDto>>, IProjectPermissionRequest;
