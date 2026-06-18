namespace BuildingBlocks.Application.Authorization;

public interface IProjectPermissionService
{
    Task<ProjectPermissionCheckStatus> CheckPermissionAsync(
        Guid projectId,
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken = default);

    Task RequestApprovalAsync(
        Guid projectId,
        Guid userId,
        string permissionName,
        string actionName,
        string actionPayload,
        CancellationToken cancellationToken = default);
}
