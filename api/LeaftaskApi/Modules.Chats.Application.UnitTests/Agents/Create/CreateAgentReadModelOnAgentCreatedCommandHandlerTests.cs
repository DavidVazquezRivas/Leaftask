using FluentAssertions;
using Modules.Chats.Application.Agents.Create;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Repositories;
using NSubstitute;

namespace Modules.Chats.Application.UnitTests.Agents.Create;

public class CreateAgentReadModelOnAgentCreatedCommandHandlerTests
{
    private readonly CreateAgentReadModelOnAgentCreatedCommandHandler _handler;
    private readonly IAgentReadModelRepository _agentReadModelRepositoryMock;

    public CreateAgentReadModelOnAgentCreatedCommandHandlerTests()
    {
        _agentReadModelRepositoryMock = Substitute.For<IAgentReadModelRepository>();
        _handler = new CreateAgentReadModelOnAgentCreatedCommandHandler(_agentReadModelRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_CreateAgentReadModel_When_NotExists()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        CreateAgentReadModelOnAgentCreatedCommand command = new(agentId, "Daily Standup Bot");

        _agentReadModelRepositoryMock.ExistsByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _agentReadModelRepositoryMock.Received(1).AddAsync(
            Arg.Is<AgentReadModel>(a => a.Id == agentId && a.Name == "Daily Standup Bot"),
            Arg.Any<CancellationToken>());
        await _agentReadModelRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Skip_When_AlreadyExists()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        CreateAgentReadModelOnAgentCreatedCommand command = new(agentId, "PM Reporter");

        _agentReadModelRepositoryMock.ExistsByIdAsync(agentId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _agentReadModelRepositoryMock.DidNotReceive().AddAsync(
            Arg.Any<AgentReadModel>(), Arg.Any<CancellationToken>());
        await _agentReadModelRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
