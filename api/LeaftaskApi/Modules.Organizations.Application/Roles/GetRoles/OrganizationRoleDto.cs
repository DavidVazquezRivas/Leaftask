namespace Modules.Organizations.Application.Roles.GetRoles;

public sealed record OrganizationRoleDto(
    Guid Id,
    string Name,
    int TotalMembers,
    IReadOnlyCollection<OrganizationRolePermissionDto> Permissions);
