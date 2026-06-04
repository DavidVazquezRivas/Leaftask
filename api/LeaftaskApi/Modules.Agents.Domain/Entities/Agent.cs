using BuildingBlocks.Domain.Entities;
using Modules.Agents.Domain.Entities.AgentTriggers;
using Modules.Agents.Domain.Entities.Model;

namespace Modules.Agents.Domain.Entities;

public sealed class Agent : Entity
{
    private readonly List<AgentEventTrigger> _eventTriggers = [];
    private readonly List<AgentTimeTrigger> _timeTriggers = [];

    private Agent() { }

    private Agent(
        Guid id,
        Guid projectId,
        string name,
        string instructions,
        string systemPrompt,
        ModelConfig modelConfig,
        Guid? templateId,
        DateTime createdAt)
    {
        Id = id;
        ProjectId = projectId;
        Name = name;
        Instructions = instructions;
        SystemPrompt = systemPrompt;
        ModelConfig = modelConfig;
        TemplateId = templateId;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public Guid ProjectId { get; }
    public string Name { get; }
    public string Instructions { get; }
    public string SystemPrompt { get; }
    public ModelConfig ModelConfig { get; } = null!;
    public Guid? TemplateId { get; }
    public DateTime CreatedAt { get; }

    public IReadOnlyList<AgentEventTrigger> EventTriggers => _eventTriggers;
    public IReadOnlyList<AgentTimeTrigger> TimeTriggers => _timeTriggers;

    public static Agent Create(
        Guid id,
        Guid projectId,
        string name,
        string instructions,
        string systemPrompt,
        ModelConfig modelConfig,
        Guid? templateId,
        DateTime createdAt,
        IEnumerable<(string EventType, string UserPrompt)> eventTriggers,
        IEnumerable<(string Name, string CronExpression, string TimeZone)> timeTriggers)
    {
        Agent agent = new(id, projectId, name, instructions, systemPrompt, modelConfig, templateId, createdAt);

        foreach ((string eventType, string userPrompt) in eventTriggers)
            agent._eventTriggers.Add(new AgentEventTrigger(Guid.NewGuid(), userPrompt, eventType, agent));

        foreach ((string triggerName, string cron, string tz) in timeTriggers)
            agent._timeTriggers.Add(new AgentTimeTrigger(Guid.NewGuid(), triggerName, cron, tz, true, agent));

        return agent;
    }
}
