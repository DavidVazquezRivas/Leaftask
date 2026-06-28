using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Permissions.GetRoles;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Permissions.GetRoles;

public class GetProjectRolesQueryHandlerTests
{
    private readonly GetProjectRolesQueryHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IProjectAccessChecker _accessCheckerMock;
    private readonly IUserContext _userContextMock;
    private readonly IGetProjectRolesQueryService _rolesServiceMock;

    public GetProjectRolesQueryHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _accessCheckerMock = Substitute.For<IProjectAccessChecker>();
        _userContextMock = Substitute.For<IUserContext>();
        _rolesServiceMock = Substitute.For<IGetProjectRolesQueryService>();

        _handler = new GetProjectRolesQueryHandler(
            _projectRepositoryMock,
            _accessCheckerMock,
            _userContextMock,
            _rolesServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnRoles_When_ProjectExistsAndUserHasAccess()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        GetProjectRolesQuery query = new(projectId);

        Project project = ProjectTestBuilder.AProject().Build();
        _userContextMock.UserId.Returns(userId);
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _accessCheckerMock.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(true);

        IReadOnlyList<ProjectRoleDto> roles =
        [
            new ProjectRoleDto(Guid.NewGuid(), "Admin", 1, [])
        ];
        _rolesServiceMock.GetRolesAsync(projectId, Arg.Any<CancellationToken>()).Returns(roles);

        // Act
        Result<IReadOnlyList<ProjectRoleDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(roles);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        GetProjectRolesQuery query = new(projectId);

        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        // Act
        Result<IReadOnlyList<ProjectRoleDto>> result = await _handler.Handle(query, CancellationToken.None);

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
        GetProjectRolesQuery query = new(projectId);

        Project project = ProjectTestBuilder.AProject().Build();
        _userContextMock.UserId.Returns(userId);
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _accessCheckerMock.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result<IReadOnlyList<ProjectRoleDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
        await _rolesServiceMock.DidNotReceive().GetRolesAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
