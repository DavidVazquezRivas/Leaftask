using BuildingBlocks.Domain.Entities;

namespace Modules.Notification.Domain.Entities.Approval;

public sealed class RequestComment : Entity
{
    private RequestComment() { }

    private RequestComment(Guid id, Guid requestId, string content, DateTime createdAt)
    {
        Id = id;
        RequestId = requestId;
        Content = content;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public Guid RequestId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserReadModel CreatedBy { get; set; }

    public static RequestComment Create(Guid requestId, string content, UserReadModel author) =>
        new(Guid.NewGuid(), requestId, content, DateTime.UtcNow) { CreatedBy = author };
}
