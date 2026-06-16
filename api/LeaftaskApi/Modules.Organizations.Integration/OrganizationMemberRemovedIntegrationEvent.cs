using BuildingBlocks.Integration;

namespace Modules.Organizations.Integration;

public sealed record OrganizationMemberRemovedIntegrationEvent(
    Guid OrganizationId,
    Guid UserId) : IIntegrationEvent;
