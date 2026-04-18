using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.DrivingInfrastructure.Models.Requests;

public sealed record CreateOrganizationRoleRequest(
    string Name,
    IReadOnlyCollection<CreateOrganizationRolePermissionRequest>? Permissions = null);

public sealed record CreateOrganizationRolePermissionRequest(
    Guid Id,
    PermissionLevel Level);
