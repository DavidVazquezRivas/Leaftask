using BuildingBlocks.Domain.Entities;

namespace Modules.Agents.Domain.Entities.AgentTriggers;

public sealed class AgentEventTrigger : Entity
{
    private AgentEventTrigger() { }

    public AgentEventTrigger(Guid id, string userPrompt, string @event, Agent agent)
    {
        Id = id;
        UserPrompt = userPrompt;
        Event = @event;
        Agent = agent;
    }

    public Guid Id { get; }
    public string UserPrompt { get; }
    public string Event { get; }
    public Agent Agent { get; } = null!;
}
