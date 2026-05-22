namespace Modules.Projects.Application.Permissions.GetPermissions;

public interface IGetProjectPermissionsQueryService
{
    Task<IReadOnlyList<ProjectPermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken = default);
}
