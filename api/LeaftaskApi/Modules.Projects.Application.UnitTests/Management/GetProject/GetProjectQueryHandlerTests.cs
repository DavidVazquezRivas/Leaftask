using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Management.Create;
using Modules.Projects.Application.Management.GetProject;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Management.GetProject;

public class GetProjectQueryHandlerTests
{
    private readonly GetProjectQueryHandler handler;
    private readonly IProjectRepository projectRepository;
    private readonly IProjectAccessChecker accessChecker;
    private readonly IUserContext userContext;

    public GetProjectQueryHandlerTests()
    {
        projectRepository = Substitute.For<IProjectRepository>();
        accessChecker = Substitute.For<IProjectAccessChecker>();
        userContext = Substitute.For<IUserContext>();

        handler = new GetProjectQueryHandler(projectRepository, accessChecker, userContext);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_AccessCheckerAllows()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        userContext.UserId.Returns(userId);
        Project project = ProjectTestBuilder.AProject().OwnedByUser(userId).Build();

        projectRepository.GetByIdAsync(project.Id, Arg.Any<CancellationToken>()).Returns(project);
        accessChecker.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Result<ProjectResponse> result = await handler.Handle(new GetProjectQuery(project.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(project.Id);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectDoesNotExist()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        userContext.UserId.Returns(Guid.NewGuid());
        projectRepository.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns((Project?)null);

        // Act
        Result<ProjectResponse> result = await handler.Handle(new GetProjectQuery(projectId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AccessCheckerDenies()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        userContext.UserId.Returns(userId);
        Project project = ProjectTestBuilder.AProject().OwnedByUser(Guid.NewGuid()).Build();

        projectRepository.GetByIdAsync(project.Id, Arg.Any<CancellationToken>()).Returns(project);
        accessChecker.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result<ProjectResponse> result = await handler.Handle(new GetProjectQuery(project.Id), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
    }
}
