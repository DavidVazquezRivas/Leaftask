namespace Modules.Organizations.Application.Roles.Create;

public sealed record OrganizationRoleResponse(
    Guid Id,
    string Name,
    int TotalMembers,
    IReadOnlyCollection<CreateOrganizationRolePermissionResponse> Permissions);
