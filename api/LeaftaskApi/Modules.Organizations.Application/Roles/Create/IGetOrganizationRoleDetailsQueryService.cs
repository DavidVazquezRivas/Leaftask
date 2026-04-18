namespace Modules.Organizations.Application.Roles.Create;

public interface IGetOrganizationRoleDetailsQueryService
{
    Task<OrganizationRoleResponse?> GetOrganizationRoleAsync(
        Guid organizationId,
        Guid roleId,
        CancellationToken cancellationToken = default);
}
