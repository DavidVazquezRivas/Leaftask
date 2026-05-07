namespace Modules.Organizations.Integration;

public interface IOrganizationPermissionService
{
    Task<OrganizationPermissionCheckStatus> CheckPermissionAsync(
        Guid organizationId,
        Guid userId,
        string permissionName,
        CancellationToken cancellationToken = default);

    Task<bool> IsOrganizationMemberAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
