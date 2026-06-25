using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Invitations.GetPending;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Invitations.GetPending;

public class GetPendingProjectInvitationsQueryHandlerTests
{
    private readonly GetPendingProjectInvitationsQueryHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IProjectAccessChecker _accessCheckerMock;
    private readonly IUserContext _userContextMock;
    private readonly IGetPendingProjectInvitationsQueryService _queryServiceMock;

    public GetPendingProjectInvitationsQueryHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _accessCheckerMock = Substitute.For<IProjectAccessChecker>();
        _userContextMock = Substitute.For<IUserContext>();
        _queryServiceMock = Substitute.For<IGetPendingProjectInvitationsQueryService>();

        _handler = new GetPendingProjectInvitationsQueryHandler(
            _projectRepositoryMock,
            _accessCheckerMock,
            _userContextMock,
            _queryServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnInvitations_When_ProjectExistsAndUserHasAccess()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        GetPendingProjectInvitationsQuery query = new(projectId);

        Project project = ProjectTestBuilder.AProject().Build();
        _userContextMock.UserId.Returns(userId);
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _accessCheckerMock.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(true);

        IReadOnlyList<ProjectInvitationDto> invitations =
        [
            new ProjectInvitationDto(
                Guid.NewGuid(),
                new ProjectInvitationUserDto(Guid.NewGuid(), "John Doe", "john@example.com"),
                new ProjectInvitationRoleDto(Guid.NewGuid(), "Member"))
        ];
        _queryServiceMock.GetPendingAsync(projectId, Arg.Any<CancellationToken>()).Returns(invitations);

        // Act
        Result<IReadOnlyList<ProjectInvitationDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        GetPendingProjectInvitationsQuery query = new(projectId);

        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        // Act
        Result<IReadOnlyList<ProjectInvitationDto>> result = await _handler.Handle(query, CancellationToken.None);

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
        GetPendingProjectInvitationsQuery query = new(projectId);

        Project project = ProjectTestBuilder.AProject().Build();
        _userContextMock.UserId.Returns(userId);
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _accessCheckerMock.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result<IReadOnlyList<ProjectInvitationDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
        await _queryServiceMock.DidNotReceive().GetPendingAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
