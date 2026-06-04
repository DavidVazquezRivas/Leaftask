using BuildingBlocks.Domain.Entities;
using Modules.Agents.Domain.Entities.Model;

namespace Modules.Agents.Domain.Entities;

public sealed class Agent : Entity
{
    private Agent() { }

    public Agent(Guid id, string name, string description, string systemPrompt, ModelConfig modelConfig)
    {
        Id = id;
        Name = name;
        Description = description;
        SystemPrompt = systemPrompt;
        ModelConfig = modelConfig;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string SystemPrompt { get; }
    public ModelConfig ModelConfig { get; }
}
