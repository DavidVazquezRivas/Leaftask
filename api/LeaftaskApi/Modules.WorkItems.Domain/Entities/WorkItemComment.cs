using BuildingBlocks.Domain.Entities;

namespace Modules.WorkItems.Domain.Entities;

public sealed class WorkItemComment : Entity
{
    private WorkItemComment() { }

    public WorkItemComment(Guid id, string comment, WorkItem workItem, UserReadModel user)
    {
        Id = id;
        Content = comment;
        WorkItem = workItem;
        User = user;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public string Content { get; }
    public DateTime CreatedAt { get; }
    public WorkItem WorkItem { get; }
    public UserReadModel User { get; }
}
