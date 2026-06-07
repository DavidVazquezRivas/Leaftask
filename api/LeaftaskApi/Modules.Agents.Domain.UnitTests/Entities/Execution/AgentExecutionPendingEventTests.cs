using FluentAssertions;
using Modules.Agents.Domain.Entities.Execution;

namespace Modules.Agents.Domain.UnitTests.Entities.Execution;

public class AgentExecutionPendingEventTests
{
    [Fact]
    public void Create_Should_SetAllProperties()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Guid executionId = Guid.NewGuid();
        const string eventType = "chat.message_sent";
        const string correlationId = "some-chat-id";

        // Act
        AgentExecutionPendingEvent pending = new(id, executionId, eventType, correlationId);

        // Assert
        pending.Id.Should().Be(id);
        pending.ExecutionId.Should().Be(executionId);
        pending.EventType.Should().Be(eventType);
        pending.CorrelationId.Should().Be(correlationId);
    }

    [Fact]
    public void Create_Should_HaveIsResolvedFalse()
    {
        // Act
        AgentExecutionPendingEvent pending = new(Guid.NewGuid(), Guid.NewGuid(), "workitem.created", "project-id");

        // Assert
        pending.IsResolved.Should().BeFalse();
    }

    [Fact]
    public void Resolve_Should_SetIsResolvedToTrue()
    {
        // Arrange
        AgentExecutionPendingEvent pending = new(Guid.NewGuid(), Guid.NewGuid(), "chat.message_sent", "chat-id");

        // Act
        pending.Resolve();

        // Assert
        pending.IsResolved.Should().BeTrue();
    }

    [Fact]
    public void Resolve_Should_BeIdempotent()
    {
        // Arrange
        AgentExecutionPendingEvent pending = new(Guid.NewGuid(), Guid.NewGuid(), "chat.message_sent", "chat-id");

        // Act
        pending.Resolve();
        pending.Resolve();

        // Assert
        pending.IsResolved.Should().BeTrue();
    }
}
