using BuildingBlocks.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;

namespace Modules.WorkItems.Domain.Entities;

public sealed class WorkItem : Entity
{
    private WorkItem() { }

    private WorkItem(
        Guid id,
        int code,
        string title,
        string description,
        DateTime limitDate,
        ProjectReadModel project,
        WorkItemStatus status,
        WorkItemType type)
    {
        Id = id;
        Code = code;
        Title = title;
        Description = description;
        Progress = 0;
        LimitDate = limitDate;
        Project = project;
        Status = status;
        Type = type;
    }

    public Guid Id { get; }
    public int Code { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int Progress { get; private set; }
    public DateTime LimitDate { get; private set; }
    public ProjectReadModel Project { get; }
    public WorkItemStatus Status { get; private set; }
    public WorkItemType Type { get; private set; }
    public UserReadModel? Asignee { get; }

    public static WorkItem Create(
        int code,
        string title,
        string description,
        DateTime limitDate,
        ProjectReadModel project,
        WorkItemStatus status,
        WorkItemType type) =>
        new(Guid.NewGuid(), code, title, description, limitDate, project, status, type);
}
