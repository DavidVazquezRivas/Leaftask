namespace Modules.Organizations.Application.Management;

public interface IGetOrganizationDetailsQueryService
{
    Task<BasicOrganizationResponse?> GetOrganizationDetailsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
