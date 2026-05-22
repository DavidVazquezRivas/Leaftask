using Modules.Projects.Application.Authorization;
using Modules.Projects.Integration;
using ProjectPermissionCheckStatus = Modules.Projects.Integration.ProjectPermissionCheckStatus;

namespace Modules.Projects.DrivingInfrastructure.Services;

public sealed class ProjectPermissionService(IProjectPermissionChecker permissionChecker)
    : IProjectPermissionService
{
    public async Task<ProjectPermissionCheckStatus> CheckPermissionAsync(
        Guid projectId,
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken = default)
    {
        Application.Authorization.ProjectPermissionCheckStatus status =
            await permissionChecker.CheckAsync(projectId, userId, permissionName, cancellationToken);

        return status switch
        {
            Application.Authorization.ProjectPermissionCheckStatus.Full => ProjectPermissionCheckStatus.Full,
            Application.Authorization.ProjectPermissionCheckStatus.ProjectNotFound => ProjectPermissionCheckStatus.ProjectNotFound,
            _ => ProjectPermissionCheckStatus.Denied
        };
    }
}
