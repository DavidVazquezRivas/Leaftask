using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Members.GetDistribution;

public sealed record GetOrganizationMembersDistributionQuery(Guid OrganizationId)
    : IQuery<Result<IReadOnlyList<OrganizationMemberDistributionDto>>>;
