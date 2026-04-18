namespace Modules.Organizations.Application.Invitations;

public sealed record OrganizationInvitationResponse(
    Guid Id,
    Guid OrganizationId,
    Guid UserId,
    Guid OrganizationRoleId,
    string Status,
    DateTime InvitedAt,
    DateTime? RespondedAt,
    DateTime? AbandonedAt);
