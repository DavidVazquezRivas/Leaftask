using FluentAssertions;
using Modules.Chats.Application.Agents.Delete;
using Modules.Chats.Domain.Repositories;
using NSubstitute;

namespace Modules.Chats.Application.UnitTests.Agents.Delete;

public class DeleteAgentReadModelOnAgentDeletedCommandHandlerTests
{
    private readonly DeleteAgentReadModelOnAgentDeletedCommandHandler _handler;
    private readonly IAgentReadModelRepository _repositoryMock;

    public DeleteAgentReadModelOnAgentDeletedCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IAgentReadModelRepository>();
        _handler = new DeleteAgentReadModelOnAgentDeletedCommandHandler(_repositoryMock);
    }

    [Fact]
    public async Task Handle_Should_RemoveAgentReadModel_ByAgentId()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        DeleteAgentReadModelOnAgentDeletedCommand command = new(agentId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.Received(1).RemoveByIdAsync(agentId, Arg.Any<CancellationToken>());
    }
}
