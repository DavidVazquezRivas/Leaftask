namespace Modules.Organizations.Application.Members.GetDistribution;

public interface IGetOrganizationMembersDistributionQueryService
{
    Task<IReadOnlyList<OrganizationMemberDistributionDto>> GetOrganizationMembersDistributionAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
