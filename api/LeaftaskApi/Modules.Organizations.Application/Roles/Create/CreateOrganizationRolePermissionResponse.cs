using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Application.Roles.Create;

public sealed record CreateOrganizationRolePermissionResponse(
    Guid Id,
    PermissionLevel Level);
