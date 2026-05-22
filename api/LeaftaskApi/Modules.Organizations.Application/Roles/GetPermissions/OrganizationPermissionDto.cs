namespace Modules.Organizations.Application.Roles.GetPermissions;

public sealed record OrganizationPermissionDto(
    Guid Id,
    string Name,
    string Description);
