namespace Modules.Chats.Application.Chats;

public sealed record ChatMessageDto(
    Guid Id,
    Guid ChatId,
    string Content,
    DateTime Timestamp,
    string Status,
    ChatSenderDto? Sender);
