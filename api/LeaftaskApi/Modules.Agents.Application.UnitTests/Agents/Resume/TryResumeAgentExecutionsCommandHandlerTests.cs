using FluentAssertions;
using Modules.Agents.Application.Agents.Resume;
using Modules.Agents.Domain;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Repositories;
using Modules.Agents.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Agents.Application.UnitTests.Agents.Resume;

public class TryResumeAgentExecutionsCommandHandlerTests
{
    private readonly TryResumeAgentExecutionsCommandHandler _handler;
    private readonly IAgentExecutionPendingEventRepository _pendingEventRepositoryMock;
    private readonly IAgentExecutionRepository _executionRepositoryMock;
    private readonly IAgentExecutionMessageRepository _messageRepositoryMock;

    public TryResumeAgentExecutionsCommandHandlerTests()
    {
        _pendingEventRepositoryMock = Substitute.For<IAgentExecutionPendingEventRepository>();
        _executionRepositoryMock = Substitute.For<IAgentExecutionRepository>();
        _messageRepositoryMock = Substitute.For<IAgentExecutionMessageRepository>();

        _handler = new TryResumeAgentExecutionsCommandHandler(
            _pendingEventRepositoryMock,
            _executionRepositoryMock,
            _messageRepositoryMock);
    }

    private static AgentExecution CreateSuspendedExecution()
    {
        Domain.Entities.Agent agent = AgentTestBuilder.AnAgent().Build();
        DateTime now = DateTime.UtcNow;
        AgentExecution execution = new(Guid.NewGuid(), "{}", ExecutionStatus.Suspended, now, now, agent);
        execution.Suspend();
        return execution;
    }

    [Fact]
    public async Task Handle_Should_DoNothing_When_NoPendingEventsExist()
    {
        // Arrange
        TryResumeAgentExecutionsCommand command = new(AgentEventTypes.ChatMessageSent, "chat-123", "Hello");

        _pendingEventRepositoryMock
            .GetUnresolvedAsync(AgentEventTypes.ChatMessageSent, "chat-123", Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionPendingEvent>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _executionRepositoryMock.DidNotReceive().GetByIdTrackedAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await _executionRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_AddUserMessage_For_Each_PendingEvent()
    {
        // Arrange
        Guid executionId = Guid.NewGuid();
        AgentExecution execution = CreateSuspendedExecution();
        AgentExecutionPendingEvent pendingEvent = new(Guid.NewGuid(), executionId, AgentEventTypes.ChatMessageSent, "chat-123");

        TryResumeAgentExecutionsCommand command = new(AgentEventTypes.ChatMessageSent, "chat-123", "Here is my status update");

        _pendingEventRepositoryMock
            .GetUnresolvedAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionPendingEvent> { pendingEvent });
        _executionRepositoryMock
            .GetByIdTrackedAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(execution);
        _messageRepositoryMock
            .GetByExecutionIdAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionMessage>());
        _pendingEventRepositoryMock
            .HasUnresolvedAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _messageRepositoryMock.Received(1).AddAsync(
            Arg.Is<AgentExecutionMessage>(m =>
                m.ExecutionId == executionId
                && m.Role == MessageRole.User
                && m.Content == "Here is my status update"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ResolvePendingEvent()
    {
        // Arrange
        Guid executionId = Guid.NewGuid();
        AgentExecution execution = CreateSuspendedExecution();
        AgentExecutionPendingEvent pendingEvent = new(Guid.NewGuid(), executionId, AgentEventTypes.ChatMessageSent, "chat-123");

        _pendingEventRepositoryMock
            .GetUnresolvedAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionPendingEvent> { pendingEvent });
        _executionRepositoryMock
            .GetByIdTrackedAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(execution);
        _messageRepositoryMock
            .GetByExecutionIdAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionMessage>());
        _pendingEventRepositoryMock
            .HasUnresolvedAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await _handler.Handle(new TryResumeAgentExecutionsCommand(AgentEventTypes.ChatMessageSent, "chat-123", "OK"), CancellationToken.None);

        // Assert
        pendingEvent.IsResolved.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReQueue_Execution_When_NoMoreUnresolvedEvents()
    {
        // Arrange
        Guid executionId = Guid.NewGuid();
        AgentExecution execution = CreateSuspendedExecution();
        AgentExecutionPendingEvent pendingEvent = new(Guid.NewGuid(), executionId, AgentEventTypes.ChatMessageSent, "chat-123");

        _pendingEventRepositoryMock
            .GetUnresolvedAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionPendingEvent> { pendingEvent });
        _executionRepositoryMock
            .GetByIdTrackedAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(execution);
        _messageRepositoryMock
            .GetByExecutionIdAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionMessage>());
        _pendingEventRepositoryMock
            .HasUnresolvedAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(false);  // no more unresolved

        // Act
        await _handler.Handle(new TryResumeAgentExecutionsCommand(AgentEventTypes.ChatMessageSent, "chat-123", "Ready"), CancellationToken.None);

        // Assert
        execution.Status.Should().Be(ExecutionStatus.Pending);
    }

    [Fact]
    public async Task Handle_Should_NotReQueue_When_OtherUnresolvedEventsStillExist()
    {
        // Arrange
        Guid executionId = Guid.NewGuid();
        AgentExecution execution = CreateSuspendedExecution();
        AgentExecutionPendingEvent pendingEvent = new(Guid.NewGuid(), executionId, AgentEventTypes.ChatMessageSent, "chat-123");

        _pendingEventRepositoryMock
            .GetUnresolvedAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionPendingEvent> { pendingEvent });
        _executionRepositoryMock
            .GetByIdTrackedAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(execution);
        _messageRepositoryMock
            .GetByExecutionIdAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionMessage>());
        _pendingEventRepositoryMock
            .HasUnresolvedAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(true);  // still waiting for other members

        // Act
        await _handler.Handle(new TryResumeAgentExecutionsCommand(AgentEventTypes.ChatMessageSent, "chat-123", "I replied"), CancellationToken.None);

        // Assert
        execution.Status.Should().Be(ExecutionStatus.Suspended);
    }

    [Fact]
    public async Task Handle_Should_Skip_When_ExecutionNotFound()
    {
        // Arrange
        Guid executionId = Guid.NewGuid();
        AgentExecutionPendingEvent pendingEvent = new(Guid.NewGuid(), executionId, AgentEventTypes.ChatMessageSent, "chat-123");

        _pendingEventRepositoryMock
            .GetUnresolvedAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionPendingEvent> { pendingEvent });
        _executionRepositoryMock
            .GetByIdTrackedAsync(executionId, Arg.Any<CancellationToken>())
            .Returns((AgentExecution?)null);

        // Act
        await _handler.Handle(new TryResumeAgentExecutionsCommand(AgentEventTypes.ChatMessageSent, "chat-123", "msg"), CancellationToken.None);

        // Assert
        await _messageRepositoryMock.DidNotReceive().AddAsync(Arg.Any<AgentExecutionMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ComputeCorrectSequenceNumber_Based_On_ExistingMessages()
    {
        // Arrange
        Guid executionId = Guid.NewGuid();
        AgentExecution execution = CreateSuspendedExecution();
        AgentExecutionPendingEvent pendingEvent = new(Guid.NewGuid(), executionId, AgentEventTypes.ChatMessageSent, "chat-id");

        List<AgentExecutionMessage> existingMessages =
        [
            new(Guid.NewGuid(), executionId, MessageRole.System, "system prompt", null, 1),
            new(Guid.NewGuid(), executionId, MessageRole.User, "initial task", null, 2),
            new(Guid.NewGuid(), executionId, MessageRole.Assistant, "asking for status", null, 3)
        ];

        _pendingEventRepositoryMock
            .GetUnresolvedAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionPendingEvent> { pendingEvent });
        _executionRepositoryMock
            .GetByIdTrackedAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(execution);
        _messageRepositoryMock
            .GetByExecutionIdAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(existingMessages);
        _pendingEventRepositoryMock
            .HasUnresolvedAsync(executionId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await _handler.Handle(new TryResumeAgentExecutionsCommand(AgentEventTypes.ChatMessageSent, "chat-id", "Here's my update"), CancellationToken.None);

        // Assert
        await _messageRepositoryMock.Received(1).AddAsync(
            Arg.Is<AgentExecutionMessage>(m => m.SequenceNumber == 4),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_HandleMultiplePendingEvents_FromDifferentExecutions()
    {
        // Arrange
        Guid executionId1 = Guid.NewGuid();
        Guid executionId2 = Guid.NewGuid();
        AgentExecution execution1 = CreateSuspendedExecution();
        AgentExecution execution2 = CreateSuspendedExecution();

        AgentExecutionPendingEvent pending1 = new(Guid.NewGuid(), executionId1, AgentEventTypes.WorkItemCreated, "project-xyz");
        AgentExecutionPendingEvent pending2 = new(Guid.NewGuid(), executionId2, AgentEventTypes.WorkItemCreated, "project-xyz");

        _pendingEventRepositoryMock
            .GetUnresolvedAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionPendingEvent> { pending1, pending2 });
        _executionRepositoryMock.GetByIdTrackedAsync(executionId1, Arg.Any<CancellationToken>()).Returns(execution1);
        _executionRepositoryMock.GetByIdTrackedAsync(executionId2, Arg.Any<CancellationToken>()).Returns(execution2);
        _messageRepositoryMock
            .GetByExecutionIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<AgentExecutionMessage>());
        _pendingEventRepositoryMock.HasUnresolvedAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        await _handler.Handle(new TryResumeAgentExecutionsCommand(AgentEventTypes.WorkItemCreated, "project-xyz", "{}"), CancellationToken.None);

        // Assert
        await _messageRepositoryMock.Received(2).AddAsync(Arg.Any<AgentExecutionMessage>(), Arg.Any<CancellationToken>());
        execution1.Status.Should().Be(ExecutionStatus.Pending);
        execution2.Status.Should().Be(ExecutionStatus.Pending);
        await _executionRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
