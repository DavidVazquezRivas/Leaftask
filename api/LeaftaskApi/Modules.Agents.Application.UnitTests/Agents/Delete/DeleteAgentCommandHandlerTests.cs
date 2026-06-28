using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Agents.Application.Agents;
using Modules.Agents.Application.Agents.Delete;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Entities.Model;
using Modules.Agents.Domain.Errors;
using Modules.Agents.Domain.Repositories;
using NSubstitute;

namespace Modules.Agents.Application.UnitTests.Agents.Delete;

public class DeleteAgentCommandHandlerTests
{
    private readonly DeleteAgentCommandHandler _handler;
    private readonly IAgentRepository _agentRepositoryMock;
    private readonly IAgentScheduler _agentSchedulerMock;

    public DeleteAgentCommandHandlerTests()
    {
        _agentRepositoryMock = Substitute.For<IAgentRepository>();
        _agentSchedulerMock = Substitute.For<IAgentScheduler>();
        _handler = new DeleteAgentCommandHandler(_agentRepositoryMock, _agentSchedulerMock);
    }

    private static Agent CreateAgent(Guid agentId, Guid projectId)
    {
        ModelProvider provider = new(Guid.NewGuid(), "OpenAI", string.Empty);
        Model model = new(Guid.NewGuid(), "gpt-4o", provider, "GPT-4o", 0.5);
        ModelConfig config = new(Guid.NewGuid(), model, 0.7, 1024);

        return Agent.Create(
            agentId,
            projectId,
            "Test Agent",
            "Test instructions",
            "You are a test agent",
            config,
            null,
            DateTime.UtcNow,
            Guid.NewGuid(),
            [],
            []);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_And_RemoveAgent()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Agent agent = CreateAgent(agentId, projectId);
        agent.ClearDomainEvents();

        _agentRepositoryMock.GetByIdTrackedAsync(agentId, Arg.Any<CancellationToken>()).Returns(agent);
        DeleteAgentCommand command = new(agentId, projectId);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _agentRepositoryMock.Received(1).RemoveAsync(agent, Arg.Any<CancellationToken>());
        await _agentRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AgentNotFound()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        _agentRepositoryMock.GetByIdTrackedAsync(agentId, Arg.Any<CancellationToken>()).Returns((Agent?)null);
        DeleteAgentCommand command = new(agentId, Guid.NewGuid());

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AgentErrors.AgentNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AgentBelongsToDifferentProject()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        Guid actualProjectId = Guid.NewGuid();
        Guid wrongProjectId = Guid.NewGuid();
        Agent agent = CreateAgent(agentId, actualProjectId);

        _agentRepositoryMock.GetByIdTrackedAsync(agentId, Arg.Any<CancellationToken>()).Returns(agent);
        DeleteAgentCommand command = new(agentId, wrongProjectId);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AgentErrors.AgentNotFound);
    }
}
