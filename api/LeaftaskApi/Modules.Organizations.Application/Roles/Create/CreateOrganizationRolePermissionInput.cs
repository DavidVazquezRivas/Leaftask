using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Application.Roles.Create;

public sealed record CreateOrganizationRolePermissionInput(
    Guid OrganizationPermissionId,
    PermissionLevel Level);
