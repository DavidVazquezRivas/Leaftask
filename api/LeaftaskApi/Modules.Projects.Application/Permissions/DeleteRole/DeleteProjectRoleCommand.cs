using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;

namespace Modules.Projects.Application.Permissions.DeleteRole;

[RequireProjectPermission("project.roles")]
public sealed record DeleteProjectRoleCommand(Guid ProjectId, Guid RoleId)
    : ICommand<Result>, IProjectPermissionRequest;
