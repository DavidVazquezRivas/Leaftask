namespace Modules.Organizations.Application.Invitations.Respond;

public sealed record RespondOrganizationInvitationRequest
{
    public required string Status { get; init; }
}
