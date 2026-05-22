using FluentAssertions;
using Modules.WorkItems.Application.Projects.Delete;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Projects.Delete;

public class DeleteProjectReadModelOnProjectDeletedCommandHandlerTests
{
    private readonly DeleteProjectReadModelOnProjectDeletedCommandHandler _handler;
    private readonly IProjectReadModelRepository _projectReadModelRepositoryMock;
    private readonly IWorkItemRepository _workItemRepositoryMock;

    public DeleteProjectReadModelOnProjectDeletedCommandHandlerTests()
    {
        _projectReadModelRepositoryMock = Substitute.For<IProjectReadModelRepository>();
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();

        _handler = new DeleteProjectReadModelOnProjectDeletedCommandHandler(
            _projectReadModelRepositoryMock,
            _workItemRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_RemoveProjectReadModel_When_Exists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        DeleteProjectReadModelOnProjectDeletedCommand command = new(projectId);
        ProjectReadModel projectReadModel = new(projectId, "TST");

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(projectReadModel);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _projectReadModelRepositoryMock.Received(1).Remove(projectReadModel);
        await _workItemRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
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
        await _workItemRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
