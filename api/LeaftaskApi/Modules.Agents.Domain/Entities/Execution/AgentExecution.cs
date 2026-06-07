using BuildingBlocks.Domain.Entities;

namespace Modules.Agents.Domain.Entities.Execution;

public sealed class AgentExecution : Entity
{
    private readonly List<AgentExecutionPendingEvent> _pendingEvents = [];

    private AgentExecution() { }

    public AgentExecution(Guid id, string payload, ExecutionStatus status, DateTime createdAt, DateTime updatedAt,
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
    public ExecutionStatus Status { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public Agent Agent { get; } = null!;

    public IReadOnlyList<AgentExecutionPendingEvent> PendingEvents => _pendingEvents;

    public void Start()
    {
        Status = ExecutionStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = ExecutionStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = ExecutionStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        Status = ExecutionStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReQueue()
    {
        Status = ExecutionStatus.Pending;
        UpdatedAt = DateTime.UtcNow;
    }
}
