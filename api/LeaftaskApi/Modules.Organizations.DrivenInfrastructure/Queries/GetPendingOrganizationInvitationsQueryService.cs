using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Application.Invitations;
using Modules.Organizations.Application.Invitations.GetPending;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Queries;

public sealed class GetPendingOrganizationInvitationsQueryService(OrganizationDbContext dbContext)
    : IGetPendingOrganizationInvitationsQueryService
{
    public async Task<IReadOnlyList<OrganizationInvitationResponse>> GetPendingInvitationsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default) =>
        await dbContext.OrganizationInvitations
            .AsNoTracking()
            .Where(invitation => invitation.OrganizationId == organizationId && invitation.Status == InvitationStatus.Pending)
            .OrderByDescending(invitation => invitation.InvitedAt)
            .Select(invitation => new OrganizationInvitationResponse(
                invitation.Id,
                invitation.OrganizationId,
                invitation.UserId,
                invitation.OrganizationRoleId,
                "Pending",
                invitation.InvitedAt,
                invitation.RespondedAt,
                invitation.AbandonedAt))
            .ToListAsync(cancellationToken);
}
