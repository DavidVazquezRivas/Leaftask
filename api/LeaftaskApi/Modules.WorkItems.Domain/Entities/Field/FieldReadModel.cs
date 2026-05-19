using BuildingBlocks.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;

namespace Modules.WorkItems.Domain.Entities.Field;

public sealed class FieldReadModel : Entity
{
    private readonly List<WorkItemType> _appliesTo = [];

    private FieldReadModel() { }

    public FieldReadModel(Guid id, string name, bool isOptional, FieldTypeReadModel fieldType)
    {
        Id = id;
        Name = name;
        IsOptional = isOptional;
        FieldType = fieldType;
    }

    public Guid Id { get; }
    public string Name { get; private set; } = string.Empty;
    public bool IsOptional { get; private set; }
    public FieldTypeReadModel FieldType { get; private set; } = null!;
    public IReadOnlyList<WorkItemType> AppliesTo => _appliesTo.AsReadOnly();

    public void Update(string name, bool isOptional, FieldTypeReadModel fieldType)
    {
        Name = name;
        IsOptional = isOptional;
        FieldType = fieldType;
    }

    public void SetAppliesTo(IReadOnlyList<WorkItemType> types)
    {
        _appliesTo.Clear();
        _appliesTo.AddRange(types);
    }
}
