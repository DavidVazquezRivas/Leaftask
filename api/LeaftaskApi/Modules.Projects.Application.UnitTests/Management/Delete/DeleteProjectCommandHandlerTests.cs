using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Management.Delete;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Management.Delete;

public class DeleteProjectCommandHandlerTests
{
    private readonly DeleteProjectCommandHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IOrganizationPermissionChecker _permissionCheckerMock;
    private readonly IUserContext _userContextMock;

    public DeleteProjectCommandHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _permissionCheckerMock = Substitute.For<IOrganizationPermissionChecker>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new DeleteProjectCommandHandler(
            _projectRepositoryMock,
            _permissionCheckerMock,
            _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_PersonalProjectAndUserIsOwner()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        Project project = ProjectTestBuilder.AProject().OwnedByUser(userId).Build();

        _projectRepositoryMock.GetByIdTrackedAsync(project.Id, Arg.Any<CancellationToken>())
            .Returns(project);

        // Act
        Result result = await _handler.Handle(new DeleteProjectCommand(project.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _projectRepositoryMock.Received(1).RemoveAsync(project, Arg.Any<CancellationToken>());
        await _projectRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_OrganizationProjectAndUserIsMember()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid organizationId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        Project project = ProjectTestBuilder.AProject().OwnedByOrganization(organizationId).Build();

        _projectRepositoryMock.GetByIdTrackedAsync(project.Id, Arg.Any<CancellationToken>())
            .Returns(project);

        _permissionCheckerMock.IsMemberAsync(organizationId, userId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _handler.Handle(new DeleteProjectCommand(project.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _projectRepositoryMock.Received(1).RemoveAsync(project, Arg.Any<CancellationToken>());
        await _projectRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectDoesNotExist()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        _userContextMock.UserId.Returns(Guid.NewGuid());

        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        // Act
        Result result = await _handler.Handle(new DeleteProjectCommand(projectId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
        await _projectRepositoryMock.DidNotReceive().RemoveAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
        await _projectRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserHasNoAccess()
    {
        // Arrange
        _userContextMock.UserId.Returns(Guid.NewGuid());

        Project project = ProjectTestBuilder.AProject().OwnedByUser(Guid.NewGuid()).Build();

        _projectRepositoryMock.GetByIdTrackedAsync(project.Id, Arg.Any<CancellationToken>())
            .Returns(project);

        // Act
        Result result = await _handler.Handle(new DeleteProjectCommand(project.Id), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
        await _projectRepositoryMock.DidNotReceive().RemoveAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
        await _projectRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
