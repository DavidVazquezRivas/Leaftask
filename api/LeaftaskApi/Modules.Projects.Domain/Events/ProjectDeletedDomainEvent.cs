using BuildingBlocks.Domain.Events;

namespace Modules.Projects.Domain.Events;

public sealed record ProjectDeletedDomainEvent(Guid ProjectId) : IDomainEvent;
