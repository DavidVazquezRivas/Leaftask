using BuildingBlocks.Domain.Entities;

namespace Modules.Notification.Domain.Entities.Notification;

public sealed class Notification : Entity
{
    private Notification() { }

    private Notification(Guid id, NotificationType type, Guid contextId, Guid targetId, Guid recipientId,
        Guid? actorId, DateTime createdAt, DateTime? readAt)
    {
        Id = id;
        Type = type;
        ContextId = contextId;
        TargetId = targetId;
        RecipientId = recipientId;
        ActorId = actorId;
        CreatedAt = createdAt;
        ReadAt = readAt;
    }

    public Guid Id { get; set; }
    public NotificationType Type { get; set; }
    /// <summary>Location context for frontend navigation (project, organization).</summary>
    public Guid ContextId { get; set; }
    /// <summary>Specific element that triggered the notification (work item, invitation, comment).</summary>
    public Guid TargetId { get; set; }
    /// <summary>User who receives this notification.</summary>
    public Guid RecipientId { get; set; }
    /// <summary>User who triggered this notification (nullable — not always known from the integration event).</summary>
    public Guid? ActorId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }

    public static Notification Create(NotificationType type, Guid contextId, Guid targetId, Guid recipientId,
        Guid? actorId = null)
    {
        return new Notification(Guid.NewGuid(), type, contextId, targetId, recipientId, actorId, DateTime.UtcNow, null);
    }

    public void Read()
    {
        ReadAt = DateTime.UtcNow;
    }
}
