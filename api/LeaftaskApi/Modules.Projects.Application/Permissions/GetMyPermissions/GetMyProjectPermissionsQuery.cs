using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Permissions.GetMyPermissions;

public sealed record GetMyProjectPermissionsQuery(Guid ProjectId)
    : IQuery<Result<IReadOnlyList<ProjectPermissionLevelDto>>>;
