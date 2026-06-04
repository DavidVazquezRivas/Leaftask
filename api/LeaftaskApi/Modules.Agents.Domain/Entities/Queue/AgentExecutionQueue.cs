using BuildingBlocks.Domain.Entities;

namespace Modules.Agents.Domain.Entities.Queue;

public sealed class AgentExecutionQueue : Entity
{
    private AgentExecutionQueue() { }

    public AgentExecutionQueue(Guid id, string payload, QueueStatus status, DateTime createdAt, DateTime updatedAt,
        Agent agent)
    {
        Id = id;
        Payload = payload;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
        Agent = agent;
    }

    public Guid Id { get; }
    public string Payload { get; }
    public QueueStatus Status { get; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }
    public Agent Agent { get; } = null!;
}
