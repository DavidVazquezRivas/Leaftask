using FluentAssertions;
using Modules.Agents.Application.Projects.Delete;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Repositories;
using NSubstitute;

namespace Modules.Agents.Application.UnitTests.Projects.Delete;

public class DeleteProjectReadModelOnProjectDeletedCommandHandlerTests
{
    private readonly DeleteProjectReadModelOnProjectDeletedCommandHandler _handler;
    private readonly IProjectReadModelRepository _projectReadModelRepositoryMock;

    public DeleteProjectReadModelOnProjectDeletedCommandHandlerTests()
    {
        _projectReadModelRepositoryMock = Substitute.For<IProjectReadModelRepository>();
        _handler = new DeleteProjectReadModelOnProjectDeletedCommandHandler(_projectReadModelRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_RemoveProjectReadModel_When_Exists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        DeleteProjectReadModelOnProjectDeletedCommand command = new(projectId);
        ProjectReadModel existing = new(projectId, "TST");

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(existing);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _projectReadModelRepositoryMock.Received(1).Remove(existing);
        await _projectReadModelRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Skip_When_ProjectReadModelNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        DeleteProjectReadModelOnProjectDeletedCommand command = new(projectId);

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((ProjectReadModel?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _projectReadModelRepositoryMock.DidNotReceive().Remove(Arg.Any<ProjectReadModel>());
        await _projectReadModelRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
