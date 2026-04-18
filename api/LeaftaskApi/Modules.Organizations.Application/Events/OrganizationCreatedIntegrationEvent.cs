using BuildingBlocks.Integration;

namespace Modules.Organizations.Application.Events;

public sealed record OrganizationCreatedIntegrationEvent(
    Guid OrganizationId,
    Guid CreatorUserId) : IIntegrationEvent;
