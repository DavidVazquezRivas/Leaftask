using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Application.Roles.Patch;

public sealed record PatchOrganizationRolePermissionInput(
    Guid OrganizationPermissionId,
    PermissionLevel Level);
