using BuildingBlocks.Domain.Events;

namespace Modules.Projects.Domain.Events;

public sealed record ProjectCreatedDomainEvent(
    Guid ProjectId,
    string Abbreviation) : IDomainEvent;
