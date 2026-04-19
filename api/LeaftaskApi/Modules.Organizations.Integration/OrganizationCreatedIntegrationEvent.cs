using BuildingBlocks.Integration;

namespace Modules.Organizations.Integration;

public sealed record OrganizationCreatedIntegrationEvent(
    Guid OrganizationId,
    Guid CreatorUserId) : IIntegrationEvent;
