using BuildingBlocks.Domain.Entities;

namespace Modules.Agents.Domain.Entities;

public sealed class ProjectReadModel : Entity
{
    private ProjectReadModel() { }

    public ProjectReadModel(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
}
