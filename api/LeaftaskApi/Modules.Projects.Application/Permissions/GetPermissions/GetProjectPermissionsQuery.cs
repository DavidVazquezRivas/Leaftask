using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Permissions.GetPermissions;

public sealed record GetProjectPermissionsQuery(Guid ProjectId)
    : IQuery<Result<IReadOnlyList<ProjectPermissionDto>>>;
