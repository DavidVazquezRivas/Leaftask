using BuildingBlocks.Domain.Entities;

namespace Modules.Agents.Domain.Entities.AgentTriggers;

public sealed class AgentTimeTrigger : Entity
{
    private AgentTimeTrigger() { }

    public AgentTimeTrigger(Guid id, string name, string cronExpression, string timeZone, bool isActive, Agent agent)
    {
        Id = id;
        Name = name;
        CronExpression = cronExpression;
        TimeZone = timeZone;
        IsActive = isActive;
        Agent = agent;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string CronExpression { get; }
    public string TimeZone { get; }
    public bool IsActive { get; }
    public Agent Agent { get; } = null!;
}
