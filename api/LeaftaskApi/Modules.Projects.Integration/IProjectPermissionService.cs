namespace Modules.Projects.Integration;

public interface IProjectPermissionService
{
    Task<ProjectPermissionCheckStatus> CheckPermissionAsync(
        Guid projectId,
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken = default);
}
