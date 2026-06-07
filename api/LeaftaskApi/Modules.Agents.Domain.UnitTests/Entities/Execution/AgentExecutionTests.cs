using FluentAssertions;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.UnitTests.TestBuilders;

namespace Modules.Agents.Domain.UnitTests.Entities.Execution;

public class AgentExecutionTests
{
    private static AgentExecution CreateExecution(ExecutionStatus status = ExecutionStatus.Pending)
    {
        Domain.Entities.Agent agent = AgentTestBuilder.AnAgent().Build();
        DateTime now = DateTime.UtcNow;
        return new AgentExecution(Guid.NewGuid(), "{}", status, now, now, agent);
    }

    [Fact]
    public void Start_Should_SetStatusToProcessing()
    {
        // Arrange
        AgentExecution execution = CreateExecution(ExecutionStatus.Pending);

        // Act
        execution.Start();

        // Assert
        execution.Status.Should().Be(ExecutionStatus.Processing);
    }

    [Fact]
    public void Start_Should_UpdateUpdatedAt()
    {
        // Arrange
        DateTime before = DateTime.UtcNow.AddSeconds(-1);
        AgentExecution execution = CreateExecution(ExecutionStatus.Pending);

        // Act
        execution.Start();

        // Assert
        execution.UpdatedAt.Should().BeAfter(before);
    }

    [Fact]
    public void Complete_Should_SetStatusToCompleted()
    {
        // Arrange
        AgentExecution execution = CreateExecution(ExecutionStatus.Processing);

        // Act
        execution.Complete();

        // Assert
        execution.Status.Should().Be(ExecutionStatus.Completed);
    }

    [Fact]
    public void Fail_Should_SetStatusToFailed()
    {
        // Arrange
        AgentExecution execution = CreateExecution(ExecutionStatus.Processing);

        // Act
        execution.Fail();

        // Assert
        execution.Status.Should().Be(ExecutionStatus.Failed);
    }

    [Fact]
    public void Suspend_Should_SetStatusToSuspended()
    {
        // Arrange
        AgentExecution execution = CreateExecution(ExecutionStatus.Processing);

        // Act
        execution.Suspend();

        // Assert
        execution.Status.Should().Be(ExecutionStatus.Suspended);
    }

    [Fact]
    public void ReQueue_Should_SetStatusToPending()
    {
        // Arrange
        AgentExecution execution = CreateExecution(ExecutionStatus.Suspended);

        // Act
        execution.ReQueue();

        // Assert
        execution.Status.Should().Be(ExecutionStatus.Pending);
    }

    [Fact]
    public void Suspend_Then_ReQueue_Should_AllowReprocessing()
    {
        // Arrange
        AgentExecution execution = CreateExecution(ExecutionStatus.Processing);

        // Act
        execution.Suspend();
        execution.ReQueue();

        // Assert
        execution.Status.Should().Be(ExecutionStatus.Pending);
    }

    [Fact]
    public void PendingEvents_Should_BeEmptyByDefault()
    {
        // Act
        AgentExecution execution = CreateExecution();

        // Assert
        execution.PendingEvents.Should().BeEmpty();
    }

    [Fact]
    public void Create_Should_SetPayload_And_Timestamps()
    {
        // Arrange
        Domain.Entities.Agent agent = AgentTestBuilder.AnAgent().Build();
        DateTime now = DateTime.UtcNow;
        const string payload = "{\"workItemId\":\"abc\"}";

        // Act
        AgentExecution execution = new(Guid.NewGuid(), payload, ExecutionStatus.Pending, now, now, agent);

        // Assert
        execution.Payload.Should().Be(payload);
        execution.CreatedAt.Should().Be(now);
        execution.Agent.Should().Be(agent);
    }
}
