namespace Modules.Projects.Application.Permissions.GetRoles;

public sealed record ProjectRoleDto(
    Guid Id,
    string Name,
    int TotalMembers,
    IReadOnlyList<ProjectRolePermissionDto> Permissions);
