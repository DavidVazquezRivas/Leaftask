using BuildingBlocks.Domain.Entities;

namespace Modules.Projects.Domain.Entities.Member;

public sealed class AgentReadModel : Entity, IProjectMember
{
    private AgentReadModel() { }

    public AgentReadModel(Guid id, string name)
    {
        Id = id;
        Name = name;
    }

    public Guid Id { get; }
    public string Name { get; }
}
