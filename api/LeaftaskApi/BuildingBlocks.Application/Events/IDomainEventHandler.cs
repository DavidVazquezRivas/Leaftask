using BuildingBlocks.Domain.Events;

namespace BuildingBlocks.Application.Events;

public interface IDomainEventHandler<in T> where T : IDomainEvent;
