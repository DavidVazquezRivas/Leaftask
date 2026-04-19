namespace Modules.Organizations.Application.Roles.GetMyPermissions;

public interface IGetMyOrganizationPermissionsQueryService
{
    Task<IReadOnlyList<MyOrganizationPermissionDto>> GetMyPermissionsAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken cancellationToken = default);
}
