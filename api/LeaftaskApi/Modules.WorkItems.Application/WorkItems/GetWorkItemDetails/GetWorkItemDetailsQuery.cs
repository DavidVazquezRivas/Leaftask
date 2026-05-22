using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;

namespace Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;

[RequireProjectPermission("Access Project")]
public sealed record GetWorkItemDetailsQuery(Guid ProjectId, Guid WorkItemId)
    : IQuery<Result<WorkItemDetailDto>>, IProjectPermissionRequest;
