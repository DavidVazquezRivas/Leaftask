namespace Modules.Organizations.Application.Roles.GetRoles;

public interface IGetOrganizationRolesQueryService
{
    Task<IReadOnlyList<OrganizationRoleDto>> GetOrganizationRolesAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
