using BuildingBlocks.Domain.Entities;

namespace Modules.Agents.Domain.Entities;

public sealed class AgentTemplate : Entity
{
    private AgentTemplate() { }

    public AgentTemplate(Guid id, string name, string description, string instructions)
    {
        Id = id;
        Name = name;
        Description = description;
        Instructions = instructions;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string Instructions { get; }
}
