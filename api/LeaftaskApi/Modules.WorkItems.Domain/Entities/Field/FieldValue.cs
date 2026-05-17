using BuildingBlocks.Domain.Entities;

namespace Modules.WorkItems.Domain.Entities.Field;

public sealed class FieldValue : Entity
{
    private FieldValue() { }

    public FieldValue(Guid id, FieldReadModel field, WorkItem workItem, string value)
    {
        Id = id;
        Field = field;
        WorkItem = workItem;
        Value = value;
    }

    public Guid Id { get; }
    public FieldReadModel Field { get; }
    public WorkItem WorkItem { get; }
    public string Value { get; } = string.Empty;
}
