using BuildingBlocks.Domain.Entities;

namespace Modules.WorkItems.Domain.Entities.Properties;

public sealed class WorkItemType : Entity
{
    private WorkItemType() { }

    public WorkItemType(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
}
