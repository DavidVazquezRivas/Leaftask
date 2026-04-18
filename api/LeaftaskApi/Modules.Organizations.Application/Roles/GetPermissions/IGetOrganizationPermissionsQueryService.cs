namespace Modules.Organizations.Application.Roles.GetPermissions;

public interface IGetOrganizationPermissionsQueryService
{
    Task<IReadOnlyList<OrganizationPermissionDto>> GetPermissionsAsync(CancellationToken cancellationToken = default);
}
