using BuildingBlocks.Integration;

namespace Modules.Projects.Integration;

public sealed record FieldDeletedIntegrationEvent(Guid FieldId) : IIntegrationEvent;
