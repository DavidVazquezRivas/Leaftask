using BuildingBlocks.Integration;

namespace Modules.Organizations.Application.Events;

public sealed record OrganizationInvitationCreatedIntegrationEvent(
    Guid OrganizationInvitationId,
    Guid OrganizationId,
    Guid UserId,
    Guid OrganizationRoleId) : IIntegrationEvent;
