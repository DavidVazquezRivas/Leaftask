using System.Globalization;
using BuildingBlocks.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Events;

namespace Modules.WorkItems.Domain.Entities;

public sealed class WorkItem : Entity
{
    private WorkItem() { }

    private WorkItem(
        Guid id,
        int code,
        string title,
        string description,
        decimal estimation,
        DateTime limitDate,
        ProjectReadModel project,
        WorkItemStatus status,
        WorkItemType type,
        UserReadModel? assignee,
        Guid? parentId)
    {
        Id = id;
        Code = code;
        Title = title;
        Description = description;
        Estimation = estimation;
        Progress = 0;
        LimitDate = limitDate;
        Project = project;
        Status = status;
        Type = type;
        Asignee = assignee;
        ParentId = parentId;
    }

    public Guid Id { get; }
    public int Code { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public decimal Estimation { get; private set; }
    public int Progress { get; private set; }
    public DateTime LimitDate { get; private set; }
    public ProjectReadModel Project { get; }
    public WorkItemStatus Status { get; private set; }
    public WorkItemType Type { get; private set; }
    public UserReadModel? Asignee { get; private set; }
    public Guid? ParentId { get; private set; }

    public static WorkItem Create(
        int code,
        string title,
        string description,
        decimal estimation,
        DateTime limitDate,
        ProjectReadModel project,
        WorkItemStatus status,
        WorkItemType type,
        UserReadModel? assignee = null,
        Guid? parentId = null)
    {
        WorkItem workItem = new(Guid.NewGuid(), code, title, description, estimation, limitDate,
            project, status, type, assignee, parentId);

        workItem.Raise(new WorkItemCreatedDomainEvent(
            workItem.Id,
            project.Id,
            title,
            status.Id,
            type.Id,
            assignee?.Id));

        return workItem;
    }

    public void Delete() =>
        Raise(new WorkItemDeletedDomainEvent(Id, Project.Id));

    public WorkItemComment AddComment(string content, UserReadModel author, IReadOnlyList<Guid> mentionedUserIds)
    {
        WorkItemComment comment = new(Guid.NewGuid(), content, this, author);
        Raise(new CommentAddedDomainEvent(Id, comment.Id, author.Id, mentionedUserIds));
        Raise(new WorkItemCommentAddedDomainEvent(Id, Project.Id, comment.Id, author.Id));
        return comment;
    }

    public IReadOnlyList<WorkItemChange> ApplyUpdate(
        string? title,
        string? description,
        int? progress,
        decimal? estimation,
        DateTime? limitDate,
        WorkItemStatus? status,
        WorkItemType? type,
        UserReadModel? assignee,
        bool updateAssignee,
        Guid? parentId = null,
        bool updateParent = false)
    {
        List<WorkItemChange> changes = [];

        if (title is not null && title != Title)
        {
            changes.Add(new WorkItemChange("title", Title, title));
            Title = title;
        }

        if (description is not null && description != Description)
        {
            changes.Add(new WorkItemChange("description", Description, description));
            Description = description;
        }

        if (progress.HasValue && progress.Value != Progress)
        {
            changes.Add(new WorkItemChange(
                "progress",
                Progress.ToString(CultureInfo.InvariantCulture),
                progress.Value.ToString(CultureInfo.InvariantCulture)));

            Raise(new WorkItemProgressUpdatedDomainEvent(Id, Project.Id, Progress, progress.Value));
            Progress = progress.Value;
        }

        if (estimation.HasValue && estimation.Value != Estimation)
        {
            changes.Add(new WorkItemChange(
                "estimation",
                Estimation.ToString(CultureInfo.InvariantCulture),
                estimation.Value.ToString(CultureInfo.InvariantCulture)));
            Estimation = estimation.Value;
        }

        if (limitDate.HasValue && limitDate.Value.Date != LimitDate.Date)
        {
            changes.Add(new WorkItemChange(
                "limitDate",
                LimitDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                limitDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
            LimitDate = limitDate.Value;
        }

        if (status is not null && status.Id != Status.Id)
        {
            changes.Add(new WorkItemChange("statusId", Status.Id.ToString("D"), status.Id.ToString("D")));
            Raise(new WorkItemStatusChangedDomainEvent(Id, Project.Id, Status.Id, status.Id));
            Status = status;
        }

        if (type is not null && type.Id != Type.Id)
        {
            changes.Add(new WorkItemChange("typeId", Type.Id.ToString("D"), type.Id.ToString("D")));
            Type = type;
        }

        if (updateAssignee)
        {
            Guid? oldId = Asignee?.Id;
            Guid? newId = assignee?.Id;
            if (oldId != newId)
            {
                changes.Add(new WorkItemChange(
                    "assigneeId",
                    oldId?.ToString("D") ?? string.Empty,
                    newId?.ToString("D") ?? string.Empty));
                Raise(new WorkItemAssigneeChangedDomainEvent(Id, Project.Id, oldId, newId));
                Asignee = assignee;
            }
        }

        if (updateParent && parentId != ParentId)
        {
            changes.Add(new WorkItemChange(
                "parentId",
                ParentId?.ToString("D") ?? string.Empty,
                parentId?.ToString("D") ?? string.Empty));
            ParentId = parentId;
        }

        return changes;
    }
}

public sealed record WorkItemChange(string FieldName, string OldValue, string NewValue);
