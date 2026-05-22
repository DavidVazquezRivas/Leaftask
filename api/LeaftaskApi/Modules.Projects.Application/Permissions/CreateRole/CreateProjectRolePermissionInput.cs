using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.Application.Permissions.CreateRole;

public sealed record CreateProjectRolePermissionInput(Guid PermissionId, PermissionLevel Level);
