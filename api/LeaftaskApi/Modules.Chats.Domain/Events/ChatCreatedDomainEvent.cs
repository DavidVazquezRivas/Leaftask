using BuildingBlocks.Domain.Events;

namespace Modules.Chats.Domain.Events;

public sealed record ChatCreatedDomainEvent(
    Guid ChatId) : IDomainEvent;
