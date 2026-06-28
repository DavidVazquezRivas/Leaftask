using FluentAssertions;
using Modules.WorkItems.Application.Agents.Delete;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Agents.Delete;

public class DeleteAgentReadModelOnAgentDeletedCommandHandlerTests
{
    private readonly DeleteUserReadModelOnAgentDeletedCommandHandler _handler;
    private readonly IUserReadModelRepository _repositoryMock;
    private readonly IWorkItemRepository _workItemRepositoryMock;

    public DeleteAgentReadModelOnAgentDeletedCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IUserReadModelRepository>();
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _handler = new DeleteUserReadModelOnAgentDeletedCommandHandler(_repositoryMock, _workItemRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_RemoveAgentReadModel_When_Exists()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        UserReadModel entry = new(agentId, "My Agent", string.Empty);
        _repositoryMock.GetByIdAsync(agentId, Arg.Any<CancellationToken>()).Returns(entry);
        DeleteUserReadModelOnAgentDeletedCommand command = new(agentId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.Received(1).Remove(entry);
        await _workItemRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_NotRemove_When_NotExists()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        _repositoryMock.GetByIdAsync(agentId, Arg.Any<CancellationToken>()).Returns((UserReadModel?)null);
        DeleteUserReadModelOnAgentDeletedCommand command = new(agentId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repositoryMock.DidNotReceive().Remove(Arg.Any<UserReadModel>());
        await _workItemRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
