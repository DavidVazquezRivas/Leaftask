using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;
using Modules.Projects.Application.Permissions.GetRoles;

namespace Modules.Projects.Application.Permissions.CreateRole;

[RequireProjectPermission("project.roles")]
public sealed record CreateProjectRoleCommand(
    Guid ProjectId,
    string Name,
    IReadOnlyList<CreateProjectRolePermissionInput> Permissions)
    : ICommand<Result<ProjectRoleDto>>, IProjectPermissionRequest;
