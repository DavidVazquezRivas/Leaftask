namespace Modules.Organizations.Application.Invitations.GetPending;

public interface IGetPendingOrganizationInvitationsQueryService
{
    Task<IReadOnlyList<OrganizationInvitationResponse>> GetPendingInvitationsAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default);
}
