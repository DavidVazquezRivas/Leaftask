using BuildingBlocks.Domain.Entities;

namespace Modules.WorkItems.Domain.Entities;

public sealed class ActivityLog : Entity
{
    private ActivityLog() { }

    public ActivityLog(Guid id, string fieldName, string oldValue, string newValue, DateTime createdAt,
        WorkItem workItem, UserReadModel userReadModel)
    {
        Id = id;
        FieldName = fieldName;
        OldValue = oldValue;
        NewValue = newValue;
        CreatedAt = createdAt;
        WorkItem = workItem;
        UserReadModel = userReadModel;
    }

    public Guid Id { get; }
    public string FieldName { get; }
    public string OldValue { get; }
    public string NewValue { get; }
    public DateTime CreatedAt { get; }
    public WorkItem WorkItem { get; }
    public UserReadModel UserReadModel { get; }
}
