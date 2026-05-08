using BuildingBlocks.Domain.Entities;

namespace Modules.Projects.Domain.Entities.Field;

public sealed class Field : Entity
{
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

    public void UpdateType(FieldType fieldType) => FieldType = fieldType;
    public void UpdateIsOptional(bool isOptional) => IsOptional = isOptional;
}
