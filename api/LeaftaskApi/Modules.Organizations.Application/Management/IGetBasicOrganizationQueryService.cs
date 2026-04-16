namespace Modules.Organizations.Application.Management;

public interface IGetBasicOrganizationQueryService
{
    Task<BasicOrganizationResponse?> GetBasicOrganizationsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
