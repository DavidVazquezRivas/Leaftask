namespace Modules.Organizations.Application.Roles.GetMyPermissions;

public sealed record MyOrganizationPermissionDto(
    Guid Id,
    string Name,
    int Level);
