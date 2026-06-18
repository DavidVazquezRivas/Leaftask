using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;
using Modules.Projects.Application.Permissions.GetRoles;

namespace Modules.Projects.Application.Permissions.PatchRole;

[RequireProjectPermission("project.roles")]
public sealed record PatchProjectRoleCommand(
    Guid ProjectId,
    Guid RoleId,
    string? Name,
    IReadOnlyList<PatchProjectRolePermissionInput>? Permissions)
    : ICommand<Result<ProjectRoleDto>>, IProjectPermissionRequest;
