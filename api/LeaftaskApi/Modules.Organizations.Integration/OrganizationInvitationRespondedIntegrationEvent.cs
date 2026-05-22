using BuildingBlocks.Integration;

namespace Modules.Organizations.Integration;

public sealed record OrganizationInvitationRespondedIntegrationEvent(
    Guid OrganizationInvitationId,
    Guid OrganizationId,
    Guid UserId,
    Guid OrganizationRoleId,
    string Status) : IIntegrationEvent;
