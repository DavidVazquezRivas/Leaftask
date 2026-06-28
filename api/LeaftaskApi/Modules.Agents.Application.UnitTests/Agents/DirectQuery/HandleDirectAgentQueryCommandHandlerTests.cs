using FluentAssertions;
using Modules.Agents.Application.Agents.DirectQuery;
using Modules.Agents.Application.UnitTests.TestBuilders;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Entities.Execution;
using Modules.Agents.Domain.Entities.Model;
using Modules.Agents.Domain.Repositories;
using NSubstitute;

namespace Modules.Agents.Application.UnitTests.Agents.DirectQuery;

public class HandleDirectAgentQueryCommandHandlerTests
{
    private readonly HandleDirectAgentQueryCommandHandler _handler;
    private readonly IAgentRepository _agentRepositoryMock;
    private readonly IAgentExecutionRepository _executionRepositoryMock;

    public HandleDirectAgentQueryCommandHandlerTests()
    {
        _agentRepositoryMock = Substitute.For<IAgentRepository>();
        _executionRepositoryMock = Substitute.For<IAgentExecutionRepository>();
        _handler = new HandleDirectAgentQueryCommandHandler(_agentRepositoryMock, _executionRepositoryMock);
    }

    private static Agent CreateAgent(Guid agentId, Guid projectId)
    {
        ModelProvider provider = new(Guid.NewGuid(), "OpenAI", string.Empty);
        Model model = new(Guid.NewGuid(), "gpt-4o", provider, "GPT-4o", 0.5);
        ModelConfig config = new(Guid.NewGuid(), model, 0.7, 1024);

        return Agent.Create(agentId, projectId, "Test Agent", "Instructions", "System prompt", config, null, DateTime.UtcNow, Guid.NewGuid(), [], []);
    }

    [Fact]
    public async Task Handle_Should_CreateExecution_When_AgentExists_And_NoActiveExecution()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        Guid chatId = Guid.NewGuid();
        Agent agent = CreateAgent(agentId, Guid.NewGuid());

        _agentRepositoryMock.GetByIdAsync(agentId, Arg.Any<CancellationToken>()).Returns(agent);
        _executionRepositoryMock.GetActiveRegularForAgentAsync(agentId, Arg.Any<CancellationToken>())
            .Returns((AgentExecution?)null);

        HandleDirectAgentQueryCommand command = new(agentId, chatId, "What is the status?");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _executionRepositoryMock.Received(1).AddAsync(
            Arg.Is<AgentExecution>(e => e.Mode == ExecutionMode.DirectQuery),
            Arg.Any<CancellationToken>());
        await _executionRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_NotCreateExecution_When_AgentNotFound()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        _agentRepositoryMock.GetByIdAsync(agentId, Arg.Any<CancellationToken>()).Returns((Agent?)null);
        HandleDirectAgentQueryCommand command = new(agentId, Guid.NewGuid(), "Hello?");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _executionRepositoryMock.DidNotReceive().AddAsync(Arg.Any<AgentExecution>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_CreateExecution_With_ContextFromActiveExecution_When_Active()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        Agent agent = CreateAgent(agentId, Guid.NewGuid());

        AgentExecution activeExecution = new(
            Guid.NewGuid(), "{}", ExecutionStatus.Processing, DateTime.UtcNow, DateTime.UtcNow, agentId, ExecutionMode.Regular);

        _agentRepositoryMock.GetByIdAsync(agentId, Arg.Any<CancellationToken>()).Returns(agent);
        _executionRepositoryMock.GetActiveRegularForAgentAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(activeExecution);

        HandleDirectAgentQueryCommand command = new(agentId, Guid.NewGuid(), "Quick question");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _executionRepositoryMock.Received(1).AddAsync(
            Arg.Is<AgentExecution>(e => e.Mode == ExecutionMode.DirectQuery),
            Arg.Any<CancellationToken>());
    }
}
