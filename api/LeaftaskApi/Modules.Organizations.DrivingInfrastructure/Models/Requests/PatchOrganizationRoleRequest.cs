using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.DrivingInfrastructure.Models.Requests;

public sealed record PatchOrganizationRoleRequest(
    string? Name = null,
    IReadOnlyCollection<PatchOrganizationRolePermissionRequest>? Permissions = null);

public sealed record PatchOrganizationRolePermissionRequest(
    Guid Id,
    PermissionLevel Level);
