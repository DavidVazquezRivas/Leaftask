namespace Modules.Chats.Application.Chats;

public sealed record ChatDto(
    Guid Id,
    string Name,
    ChatLastMessageDto? LastMessage,
    string Type,
    ChatProjectDto? Project,
    Guid? OtherParticipantId,
    int UnreadCount);

public sealed record ChatLastMessageDto(
    string Content,
    DateTime Timestamp,
    string Status);

public sealed record ChatProjectDto(
    Guid Id,
    string Name);
