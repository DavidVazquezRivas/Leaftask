using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Permissions.GetPermissions;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Permissions.GetPermissions;

public class GetProjectPermissionsQueryHandlerTests
{
    private readonly GetProjectPermissionsQueryHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IProjectAccessChecker _accessCheckerMock;
    private readonly IUserContext _userContextMock;
    private readonly IGetProjectPermissionsQueryService _permissionsServiceMock;

    public GetProjectPermissionsQueryHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _accessCheckerMock = Substitute.For<IProjectAccessChecker>();
        _userContextMock = Substitute.For<IUserContext>();
        _permissionsServiceMock = Substitute.For<IGetProjectPermissionsQueryService>();

        _handler = new GetProjectPermissionsQueryHandler(
            _projectRepositoryMock,
            _accessCheckerMock,
            _userContextMock,
            _permissionsServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnPermissions_When_ProjectExistsAndUserHasAccess()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        GetProjectPermissionsQuery query = new(projectId);

        Project project = ProjectTestBuilder.AProject().Build();
        _userContextMock.UserId.Returns(userId);
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _accessCheckerMock.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(true);

        IReadOnlyList<ProjectPermissionDto> permissions =
        [
            new ProjectPermissionDto(Guid.NewGuid(), "project.view", "View project", "ReadOnly")
        ];
        _permissionsServiceMock.GetPermissionsAsync(Arg.Any<CancellationToken>()).Returns(permissions);

        // Act
        Result<IReadOnlyList<ProjectPermissionDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(permissions);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        GetProjectPermissionsQuery query = new(projectId);

        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        // Act
        Result<IReadOnlyList<ProjectPermissionDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserHasNoAccess()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        GetProjectPermissionsQuery query = new(projectId);

        Project project = ProjectTestBuilder.AProject().Build();
        _userContextMock.UserId.Returns(userId);
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _accessCheckerMock.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result<IReadOnlyList<ProjectPermissionDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
        await _permissionsServiceMock.DidNotReceive().GetPermissionsAsync(Arg.Any<CancellationToken>());
    }
}
