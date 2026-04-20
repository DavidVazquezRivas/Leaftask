using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Invitations.GetPending;

public sealed class GetPendingOrganizationInvitationsQueryHandler(
    IOrganizationRepository organizationRepository,
    IGetPendingOrganizationInvitationsQueryService service,
    IUserContext userContext)
    : IQueryHandler<GetPendingOrganizationInvitationsQuery, Result<IReadOnlyList<OrganizationInvitationResponse>>>
{
    public async Task<Result<IReadOnlyList<OrganizationInvitationResponse>>> Handle(
        GetPendingOrganizationInvitationsQuery request,
        CancellationToken cancellationToken)
    {
        Organization? organization = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (organization is null)
        {
            return Result.Failure<IReadOnlyList<OrganizationInvitationResponse>>(OrganizationErrors.OrganizationNotFound);
        }

        bool isMember = organization.Invitations.Any(invitation =>
            invitation.UserId == userContext.UserId && invitation.Status == InvitationStatus.Accepted);

        if (!isMember)
        {
            return Result.Failure<IReadOnlyList<OrganizationInvitationResponse>>(OrganizationErrors.OrganizationMembershipRequired);
        }

        IReadOnlyList<OrganizationInvitationResponse> invitations =
            await service.GetPendingInvitationsAsync(request.OrganizationId, cancellationToken);

        return Result.Success(invitations);
    }
}
