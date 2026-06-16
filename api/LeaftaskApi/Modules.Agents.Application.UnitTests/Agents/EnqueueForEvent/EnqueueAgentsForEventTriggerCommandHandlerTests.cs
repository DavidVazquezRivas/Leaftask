using FluentAssertions;
using Modules.Agents.Application.Agents.EnqueueForEvent;
using Modules.Agents.Domain;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Repositories;
using Modules.Agents.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Agents.Application.UnitTests.Agents.EnqueueForEvent;

public class EnqueueAgentsForEventTriggerCommandHandlerTests
{
    private readonly EnqueueAgentsForEventTriggerCommandHandler _handler;
    private readonly IAgentRepository _agentRepositoryMock;
    private readonly IAgentExecutionRepository _executionRepositoryMock;

    public EnqueueAgentsForEventTriggerCommandHandlerTests()
    {
        _agentRepositoryMock = Substitute.For<IAgentRepository>();
        _executionRepositoryMock = Substitute.For<IAgentExecutionRepository>();
        _handler = new EnqueueAgentsForEventTriggerCommandHandler(_agentRepositoryMock, _executionRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_EnqueueOneEntry_PerMatchingAgent()
    {
        // Arrange
        EnqueueAgentsForEventTriggerCommand command = new(
            AgentEventTypes.WorkItemCreated,
            "{\"workItemId\":\"123\"}");

        IReadOnlyList<Agent> agents =
        [
            AgentTestBuilder.AnAgent().Build(),
            AgentTestBuilder.AnAgent().Build()
        ];

        _agentRepositoryMock.GetByEventTriggerAsync(AgentEventTypes.WorkItemCreated, Arg.Any<CancellationToken>())
            .Returns(agents);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _executionRepositoryMock.Received(2).AddAsync(Arg.Any<AgentExecution>(), Arg.Any<CancellationToken>());
        await _executionRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_NotSave_When_NoAgentsMatch()
    {
        // Arrange
        EnqueueAgentsForEventTriggerCommand command = new(AgentEventTypes.WorkItemStatusChanged, "{}");

        _agentRepositoryMock.GetByEventTriggerAsync(AgentEventTypes.WorkItemStatusChanged, Arg.Any<CancellationToken>())
            .Returns(new List<Agent>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _executionRepositoryMock.DidNotReceive().AddAsync(Arg.Any<AgentExecution>(), Arg.Any<CancellationToken>());
        await _executionRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_EnqueueWithPendingStatus()
    {
        // Arrange
        EnqueueAgentsForEventTriggerCommand command = new(AgentEventTypes.ChatMessageSent, "{\"chatId\":\"abc\"}");

        IReadOnlyList<Agent> agents = [AgentTestBuilder.AnAgent().Build()];

        _agentRepositoryMock.GetByEventTriggerAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(agents);

        AgentExecution? capturedEntry = null;
        await _executionRepositoryMock.AddAsync(
            Arg.Do<AgentExecution>(entry => capturedEntry = entry),
            Arg.Any<CancellationToken>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedEntry.Should().NotBeNull();
        capturedEntry!.Status.Should().Be(ExecutionStatus.Pending);
        capturedEntry.Payload.Should().Be("{\"chatId\":\"abc\"}");
        capturedEntry.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_Should_EnqueueSingleAgent()
    {
        // Arrange
        EnqueueAgentsForEventTriggerCommand command = new(AgentEventTypes.WorkItemDeleted, "{}");

        IReadOnlyList<Agent> agents = [AgentTestBuilder.AnAgent().Build()];

        _agentRepositoryMock.GetByEventTriggerAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(agents);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _executionRepositoryMock.Received(1).AddAsync(Arg.Any<AgentExecution>(), Arg.Any<CancellationToken>());
        await _executionRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
