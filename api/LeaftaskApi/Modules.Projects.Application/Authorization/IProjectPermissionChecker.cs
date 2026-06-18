using BuildingBlocks.Application.Authorization;

namespace Modules.Projects.Application.Authorization;

public interface IProjectPermissionChecker
{
    Task<ProjectPermissionCheckStatus> CheckAsync(
        Guid projectId,
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken = default);
}
