using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Projects.Application.Permissions.GetRoles;

public sealed record GetProjectRolesQuery(Guid ProjectId)
    : IQuery<Result<IReadOnlyList<ProjectRoleDto>>>;
