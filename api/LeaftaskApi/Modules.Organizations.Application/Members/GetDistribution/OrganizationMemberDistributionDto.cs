namespace Modules.Organizations.Application.Members.GetDistribution;

public sealed record OrganizationMemberDistributionDto(
    Guid Id,
    int MemberCount);
