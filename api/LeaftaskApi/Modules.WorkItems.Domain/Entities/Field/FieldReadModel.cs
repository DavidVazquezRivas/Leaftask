using BuildingBlocks.Domain.Entities;

namespace Modules.WorkItems.Domain.Entities.Field;

public sealed class FieldReadModel : Entity
{
    private FieldReadModel() { }

    public FieldReadModel(Guid id, string name, bool isOptional, FieldTypeReadModel fieldType)
    {
        Id = id;
        Name = name;
        IsOptional = isOptional;
        FieldType = fieldType;
    }

    public Guid Id { get; }
    public string Name { get; }
    public bool IsOptional { get; }
    public FieldTypeReadModel FieldType { get; }
}
