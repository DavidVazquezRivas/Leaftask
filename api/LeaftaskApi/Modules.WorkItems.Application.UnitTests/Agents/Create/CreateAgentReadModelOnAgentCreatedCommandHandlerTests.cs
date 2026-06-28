using FluentAssertions;
using Modules.WorkItems.Application.Agents.Create;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Agents.Create;

public class CreateAgentReadModelOnAgentCreatedCommandHandlerTests
{
    private readonly CreateUserReadModelOnAgentCreatedCommandHandler _handler;
    private readonly IUserReadModelRepository _repositoryMock;
    private readonly IWorkItemRepository _workItemRepositoryMock;

    public CreateAgentReadModelOnAgentCreatedCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IUserReadModelRepository>();
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _handler = new CreateUserReadModelOnAgentCreatedCommandHandler(_repositoryMock, _workItemRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_CreateAgentReadModel_When_NotExists()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        _repositoryMock.ExistsByIdAsync(agentId, Arg.Any<CancellationToken>()).Returns(false);
        CreateUserReadModelOnAgentCreatedCommand command = new(agentId, "My Agent");

        UserReadModel? added = null;
        await _repositoryMock.AddAsync(Arg.Do<UserReadModel>(u => added = u), Arg.Any<CancellationToken>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        added.Should().NotBeNull();
        added!.Id.Should().Be(agentId);
        added.FirstName.Should().Be("My Agent");
        await _workItemRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_NotCreate_When_AlreadyExists()
    {
        // Arrange
        Guid agentId = Guid.NewGuid();
        _repositoryMock.ExistsByIdAsync(agentId, Arg.Any<CancellationToken>()).Returns(true);
        CreateUserReadModelOnAgentCreatedCommand command = new(agentId, "My Agent");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.DidNotReceive().AddAsync(Arg.Any<UserReadModel>(), Arg.Any<CancellationToken>());
        await _workItemRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
