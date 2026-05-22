using BuildingBlocks.Domain.Entities;
using Modules.Projects.Domain.Events;

namespace Modules.Projects.Domain.Entities.Field;

public sealed class Field : Entity
{
    private readonly List<WorkItemTypeReadModel> _appliesTo = [];

    private Field() { }

    public Field(Guid id, bool isOptional, string name, FieldType fieldType)
    {
        Id = id;
        IsOptional = isOptional;
        Name = name;
        FieldType = fieldType;
    }

    public Guid Id { get; }
    public bool IsOptional { get; private set; }
    public string Name { get; }
    public FieldType FieldType { get; private set; }
    public IReadOnlyList<WorkItemTypeReadModel> AppliesTo => _appliesTo.AsReadOnly();

    public void UpdateType(FieldType fieldType) => FieldType = fieldType;
    public void UpdateIsOptional(bool isOptional) => IsOptional = isOptional;

    public void SetAppliesTo(IReadOnlyList<WorkItemTypeReadModel> types)
    {
        _appliesTo.Clear();
        _appliesTo.AddRange(types);
    }

    public void NotifyCreated() =>
        Raise(new FieldCreatedDomainEvent(Id, Name, IsOptional, FieldType.Id, _appliesTo.Select(wt => wt.Id).ToList()));

    public void NotifyUpdated() =>
        Raise(new FieldUpdatedDomainEvent(Id, Name, IsOptional, FieldType.Id, _appliesTo.Select(wt => wt.Id).ToList()));

    public void NotifyDeleted() =>
        Raise(new FieldDeletedDomainEvent(Id));
}
