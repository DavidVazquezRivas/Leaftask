using BuildingBlocks.Domain.Entities;

namespace Modules.WorkItems.Domain.Entities.Properties;

public sealed class WorkItemStatus : Entity
{
    private WorkItemStatus() { }

    public WorkItemStatus(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
}
