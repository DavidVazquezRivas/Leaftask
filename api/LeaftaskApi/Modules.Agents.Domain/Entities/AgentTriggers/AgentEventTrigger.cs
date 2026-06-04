using BuildingBlocks.Domain.Entities;

namespace Modules.Agents.Domain.Entities.AgentTriggers;

public sealed class AgentEventTrigger : Entity
{
    private AgentEventTrigger() { }

    public AgentEventTrigger(Guid id, string userPrompt, string @event)
    {
        Id = id;
        UserPrompt = userPrompt;
        Event = @event;
    }

    public Guid Id { get; }
    public string UserPrompt { get; }
    public string Event { get; }
}
