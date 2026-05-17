using BuildingBlocks.Integration;

namespace Modules.Projects.Integration;

public sealed record ProjectCreatedIntegrationEvent(
    Guid ProjectId,
    string Abbreviation) : IIntegrationEvent;
