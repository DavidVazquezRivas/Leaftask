using BuildingBlocks.Integration;

namespace Modules.Projects.Integration;

public sealed record ProjectDeletedIntegrationEvent(Guid ProjectId) : IIntegrationEvent;
