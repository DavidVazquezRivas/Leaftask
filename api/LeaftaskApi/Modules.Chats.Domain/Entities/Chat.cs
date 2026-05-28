using BuildingBlocks.Domain.Entities;
using Modules.Chats.Domain.Events;

namespace Modules.Chats.Domain.Entities;

public sealed class Chat : Entity
{
    private Chat() { }

    private Chat(Guid id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public DateTime CreatedAt { get; }

    public static Chat Create(Guid id, DateTime createdAt)
    {
        Chat chat = new(id, createdAt);
        chat.Raise(new ChatCreatedDomainEvent(id));
        return chat;
    }
}
