using BuildingBlocks.Integration;

namespace Modules.Organizations.Integration;

public sealed record OrganizationInvitationCreatedIntegrationEvent(
    Guid OrganizationInvitationId,
    Guid OrganizationId,
    Guid UserId,
    Guid OrganizationRoleId) : IIntegrationEvent;
