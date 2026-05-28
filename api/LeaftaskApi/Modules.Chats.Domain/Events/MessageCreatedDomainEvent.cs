using BuildingBlocks.Domain.Events;

namespace Modules.Chats.Domain.Events;

public sealed record MessageCreatedDomainEvent(
    Guid MessageId,
    Guid SenderId,
    string Content) : IDomainEvent;
