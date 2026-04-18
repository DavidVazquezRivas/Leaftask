using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Members.GetDistribution;

public sealed class GetOrganizationMembersDistributionQueryHandler(
    IOrganizationRepository organizationRepository,
    IGetOrganizationMembersDistributionQueryService service,
    IUserContext userContext)
    : IQueryHandler<GetOrganizationMembersDistributionQuery, Result<IReadOnlyList<OrganizationMemberDistributionDto>>>
{
    public async Task<Result<IReadOnlyList<OrganizationMemberDistributionDto>>> Handle(
        GetOrganizationMembersDistributionQuery request,
        CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure<IReadOnlyList<OrganizationMemberDistributionDto>>(OrganizationErrors.OrganizationNotFound);
        }

        bool isMember = organization.Invitations.Any(invitation =>
            invitation.UserId == userContext.UserId && invitation.Status == InvitationStatus.Accepted);

        if (!isMember)
        {
            return Result.Failure<IReadOnlyList<OrganizationMemberDistributionDto>>(OrganizationErrors.OrganizationMembershipRequired);
        }

        IReadOnlyList<OrganizationMemberDistributionDto> distribution =
            await service.GetOrganizationMembersDistributionAsync(request.OrganizationId, cancellationToken);

        return Result.Success(distribution);
    }
}
