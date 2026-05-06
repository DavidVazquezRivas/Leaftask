using BuildingBlocks.Integration;

namespace Modules.Organizations.Integration;

public sealed record OrganizationDeletedIntegrationEvent(Guid OrganizationId) : IIntegrationEvent;
