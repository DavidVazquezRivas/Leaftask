using BuildingBlocks.Domain.Entities;

namespace Modules.WorkItems.Domain.Entities.Field;

public sealed class FieldTypeReadModel : Entity
{
    private FieldTypeReadModel() { }

    public FieldTypeReadModel(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
}
