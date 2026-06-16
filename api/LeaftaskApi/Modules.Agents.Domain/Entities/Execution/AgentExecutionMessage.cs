using BuildingBlocks.Domain.Entities;

namespace Modules.Agents.Domain.Entities.Execution;

public sealed class AgentExecutionMessage : Entity
{
    private AgentExecutionMessage() { }

    public AgentExecutionMessage(
        Guid id,
        Guid executionId,
        MessageRole role,
        string content,
        string? toolCalls,
        int sequenceNumber)
    {
        Id = id;
        ExecutionId = executionId;
        Role = role;
        Content = content;
        ToolCalls = toolCalls;
        SequenceNumber = sequenceNumber;
    }

    public Guid Id { get; }
    public Guid ExecutionId { get; }
    public MessageRole Role { get; }
    public string Content { get; }
    public string? ToolCalls { get; }
    public int SequenceNumber { get; }
}
