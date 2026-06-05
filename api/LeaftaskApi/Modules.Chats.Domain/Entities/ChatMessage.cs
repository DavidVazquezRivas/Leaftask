using BuildingBlocks.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Events;

namespace Modules.Chats.Domain.Entities;

public sealed class ChatMessage : Entity
{
    private ChatMessage() { }

    private ChatMessage(Guid id, string content, DateTime createdAt, MessageStatus status, Chat chat,
        ChatParticipant sender)
    {
        Id = id;
        Content = content;
        CreatedAt = createdAt;
        Status = status;
        Chat = chat;
        Sender = sender;
    }

    public Guid Id { get; }
    public string Content { get; }
    public DateTime CreatedAt { get; }
    public MessageStatus Status { get; private set; }
    public Chat Chat { get; } = null!;
    public ChatParticipant Sender { get; } = null!;

    public void MarkAsDelivered()
    {
        Status = MessageStatus.Delivered;
    }

    public void MarkAsRead()
    {
        Status = MessageStatus.Read;
    }

    public static ChatMessage Create(Guid id, string content, DateTime createdAt, MessageStatus status, Chat chat,
        ChatParticipant sender)
    {
        ChatMessage message = new(id, content, createdAt, status, chat, sender);
        message.Raise(new MessageCreatedDomainEvent(id, chat.Id, sender.Id, content));
        return message;
    }
}
