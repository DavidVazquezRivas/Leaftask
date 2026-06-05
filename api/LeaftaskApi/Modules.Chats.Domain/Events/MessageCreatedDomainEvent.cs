using BuildingBlocks.Domain.Events;

namespace Modules.Chats.Domain.Events;

public sealed record MessageCreatedDomainEvent(
    Guid MessageId,
    Guid ChatId,
    Guid SenderId,
    string Content) : IDomainEvent;
