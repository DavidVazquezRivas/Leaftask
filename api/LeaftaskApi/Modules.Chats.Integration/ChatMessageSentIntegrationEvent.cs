using BuildingBlocks.Integration;

namespace Modules.Chats.Integration;

public sealed record ChatMessageSentIntegrationEvent(
    Guid ChatId,
    Guid MessageId,
    Guid SenderId,
    string Content) : IIntegrationEvent;
