using BuildingBlocks.Domain.Entities;

namespace Modules.Agents.Domain.Entities.Execution;

public sealed class AgentExecutionPendingEvent : Entity
{
    private AgentExecutionPendingEvent() { }

    public AgentExecutionPendingEvent(
        Guid id,
        Guid executionId,
        string eventType,
        string correlationId)
    {
        Id = id;
        ExecutionId = executionId;
        EventType = eventType;
        CorrelationId = correlationId;
        IsResolved = false;
    }

    public Guid Id { get; }
    public Guid ExecutionId { get; }
    public string EventType { get; }
    public string CorrelationId { get; }
    public bool IsResolved { get; private set; }

    public void Resolve() => IsResolved = true;
}
