using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.Application.Permissions.PatchRole;

public sealed record PatchProjectRolePermissionInput(Guid PermissionId, PermissionLevel Level);
