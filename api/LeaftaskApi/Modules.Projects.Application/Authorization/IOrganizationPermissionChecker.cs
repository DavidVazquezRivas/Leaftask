namespace Modules.Projects.Application.Authorization;

public interface IOrganizationPermissionChecker
{
    Task<OrganizationPermissionCheckStatus> CheckAsync(
        Guid organizationId,
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken = default);

    Task<bool> IsMemberAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
