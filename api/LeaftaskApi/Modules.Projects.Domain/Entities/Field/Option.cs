using BuildingBlocks.Domain.Entities;

namespace Modules.Projects.Domain.Entities.Field;

public sealed class Option : Entity
{
    private Option() { }

    public Option(Guid id, string name, Field field)
    {
        Id = id;
        Name = name;
        Field = field;
    }

    public Guid Id { get; }
    public string Name { get; }
    public Field Field { get; }
}
