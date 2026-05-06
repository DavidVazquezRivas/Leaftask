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
    public bool IsOptional { get; }
    public string Name { get; }
    public FieldType FieldType { get; }
}
