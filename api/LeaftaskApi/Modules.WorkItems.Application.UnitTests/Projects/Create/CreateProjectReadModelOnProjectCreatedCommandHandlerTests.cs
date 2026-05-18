using FluentAssertions;
using Modules.WorkItems.Application.Projects.Create;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Projects.Create;

public class CreateProjectReadModelOnProjectCreatedCommandHandlerTests
{
    private readonly CreateProjectReadModelOnProjectCreatedCommandHandler _handler;
    private readonly IProjectReadModelRepository _projectReadModelRepositoryMock;
    private readonly IWorkItemRepository _workItemRepositoryMock;

    public CreateProjectReadModelOnProjectCreatedCommandHandlerTests()
    {
        _projectReadModelRepositoryMock = Substitute.For<IProjectReadModelRepository>();
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();

        _handler = new CreateProjectReadModelOnProjectCreatedCommandHandler(
            _projectReadModelRepositoryMock,
            _workItemRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_CreateProjectReadModel_When_NotExists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        CreateProjectReadModelOnProjectCreatedCommand command = new(projectId, "TST");

        _projectReadModelRepositoryMock.ExistsByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _projectReadModelRepositoryMock.Received(1)
            .AddAsync(Arg.Is<ProjectReadModel>(p => p.Id == projectId && p.Abbreviation == "TST"),
                Arg.Any<CancellationToken>());
        await _workItemRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Skip_When_ProjectReadModelAlreadyExists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        CreateProjectReadModelOnProjectCreatedCommand command = new(projectId, "TST");

        _projectReadModelRepositoryMock.ExistsByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _projectReadModelRepositoryMock.DidNotReceive()
            .AddAsync(Arg.Any<ProjectReadModel>(), Arg.Any<CancellationToken>());
        await _workItemRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
