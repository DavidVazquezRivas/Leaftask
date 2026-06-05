using FluentAssertions;
using Modules.Agents.Application.Projects.Create;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Repositories;
using NSubstitute;

namespace Modules.Agents.Application.UnitTests.Projects.Create;

public class CreateProjectReadModelOnProjectCreatedCommandHandlerTests
{
    private readonly CreateProjectReadModelOnProjectCreatedCommandHandler _handler;
    private readonly IProjectReadModelRepository _projectReadModelRepositoryMock;

    public CreateProjectReadModelOnProjectCreatedCommandHandlerTests()
    {
        _projectReadModelRepositoryMock = Substitute.For<IProjectReadModelRepository>();
        _handler = new CreateProjectReadModelOnProjectCreatedCommandHandler(_projectReadModelRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_CreateProjectReadModel_When_NotExists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        CreateProjectReadModelOnProjectCreatedCommand command = new(projectId, "TST");

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((ProjectReadModel?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _projectReadModelRepositoryMock.Received(1)
            .AddAsync(Arg.Is<ProjectReadModel>(p => p.Id == projectId && p.Name == "TST"),
                Arg.Any<CancellationToken>());
        await _projectReadModelRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Skip_When_ProjectReadModelAlreadyExists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        CreateProjectReadModelOnProjectCreatedCommand command = new(projectId, "TST");
        ProjectReadModel existing = new(projectId, "TST");

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(existing);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _projectReadModelRepositoryMock.DidNotReceive()
            .AddAsync(Arg.Any<ProjectReadModel>(), Arg.Any<CancellationToken>());
        await _projectReadModelRepositoryMock.DidNotReceive()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
