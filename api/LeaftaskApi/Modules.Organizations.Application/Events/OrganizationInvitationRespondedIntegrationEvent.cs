using BuildingBlocks.Integration;

namespace Modules.Organizations.Application.Events;

public sealed record OrganizationInvitationRespondedIntegrationEvent(
    Guid OrganizationInvitationId,
    Guid OrganizationId,
    Guid UserId,
    Guid OrganizationRoleId,
    string Status) : IIntegrationEvent;
