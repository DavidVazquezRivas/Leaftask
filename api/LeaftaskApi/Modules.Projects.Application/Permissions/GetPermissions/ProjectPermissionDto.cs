namespace Modules.Projects.Application.Permissions.GetPermissions;

public sealed record ProjectPermissionDto(
    Guid Id,
    string Name,
    string Description,
    string PermissionType);
