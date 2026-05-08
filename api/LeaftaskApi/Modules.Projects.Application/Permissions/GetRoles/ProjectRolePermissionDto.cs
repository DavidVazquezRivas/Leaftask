using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.Application.Permissions.GetRoles;

public sealed record ProjectRolePermissionDto(
    Guid PermissionId,
    string PermissionName,
    string PermissionType,
    PermissionLevel Level);
