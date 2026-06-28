using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Authorization;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Invitations.UpdateStatus;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Invitation;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Invitations.UpdateStatus;

public class UpdateProjectInvitationStatusCommandHandlerTests
{
    private readonly UpdateProjectInvitationStatusCommandHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IProjectRoleRepository _roleRepositoryMock;
    private readonly IProjectMemberRepository _memberRepositoryMock;
    private readonly IProjectInvitationRepository _invitationRepositoryMock;
    private readonly IProjectPermissionChecker _permissionCheckerMock;
    private readonly IUserContext _userContextMock;

    public UpdateProjectInvitationStatusCommandHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _roleRepositoryMock = Substitute.For<IProjectRoleRepository>();
        _memberRepositoryMock = Substitute.For<IProjectMemberRepository>();
        _invitationRepositoryMock = Substitute.For<IProjectInvitationRepository>();
        _permissionCheckerMock = Substitute.For<IProjectPermissionChecker>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new UpdateProjectInvitationStatusCommandHandler(
            _projectRepositoryMock,
            _roleRepositoryMock,
            _memberRepositoryMock,
            _invitationRepositoryMock,
            _permissionCheckerMock,
            _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_InvitationNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid invitationId = Guid.NewGuid();
        UpdateProjectInvitationStatusCommand command = new(projectId, invitationId, "ACCEPTED");

        _invitationRepositoryMock.GetByIdTrackedAsync(projectId, invitationId, Arg.Any<CancellationToken>())
            .Returns((ProjectInvitation?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.InvitationNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RespondingToOtherUsersInvitation()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid inviteeId = Guid.NewGuid();
        Guid otherUserId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        Guid invitationId = Guid.NewGuid();
        UpdateProjectInvitationStatusCommand command = new(projectId, invitationId, "ACCEPTED");

        ProjectInvitation invitation = ProjectInvitation.Create(projectId, inviteeId, MemberType.User, roleId);
        _invitationRepositoryMock.GetByIdTrackedAsync(projectId, invitationId, Arg.Any<CancellationToken>())
            .Returns(invitation);
        _userContextMock.UserId.Returns(otherUserId);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.InvitationAccessDenied);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_InvitationIsAccepted()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid inviteeId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        Guid invitationId = Guid.NewGuid();
        UpdateProjectInvitationStatusCommand command = new(projectId, invitationId, "ACCEPTED");

        ProjectInvitation invitation = ProjectInvitation.Create(projectId, inviteeId, MemberType.User, roleId);
        _invitationRepositoryMock.GetByIdTrackedAsync(projectId, invitationId, Arg.Any<CancellationToken>())
            .Returns(invitation);
        _userContextMock.UserId.Returns(inviteeId);

        Project project = ProjectTestBuilder.AProject().Build();
        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);

        ProjectRole role = new(roleId, "Member", project);
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>()).Returns(role);
        _roleRepositoryMock.GetRolePermissionsTrackedAsync(role.Id, Arg.Any<CancellationToken>()).Returns([]);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _memberRepositoryMock.Received(1).AddAsync(Arg.Any<ProjectMember>(), Arg.Any<CancellationToken>());
        await _invitationRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_InvitationIsRejected()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid inviteeId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        Guid invitationId = Guid.NewGuid();
        UpdateProjectInvitationStatusCommand command = new(projectId, invitationId, "REJECTED");

        ProjectInvitation invitation = ProjectInvitation.Create(projectId, inviteeId, MemberType.User, roleId);
        _invitationRepositoryMock.GetByIdTrackedAsync(projectId, invitationId, Arg.Any<CancellationToken>())
            .Returns(invitation);
        _userContextMock.UserId.Returns(inviteeId);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _memberRepositoryMock.DidNotReceive().AddAsync(Arg.Any<ProjectMember>(), Arg.Any<CancellationToken>());
        await _invitationRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_InvitationIsCancelledByAuthorizedUser()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid inviteeId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Guid invitationId = Guid.NewGuid();
        UpdateProjectInvitationStatusCommand command = new(projectId, invitationId, "CANCELLED");

        ProjectInvitation invitation = ProjectInvitation.Create(projectId, inviteeId, MemberType.User, roleId);
        _invitationRepositoryMock.GetByIdTrackedAsync(projectId, invitationId, Arg.Any<CancellationToken>())
            .Returns(invitation);
        _userContextMock.UserId.Returns(userId);
        _permissionCheckerMock.CheckAsync(
            projectId, userId, "project.invite-members", Arg.Any<CancellationToken>())
            .Returns(ProjectPermissionCheckStatus.Full);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _invitationRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CancellingWithoutPermission()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid inviteeId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Guid invitationId = Guid.NewGuid();
        UpdateProjectInvitationStatusCommand command = new(projectId, invitationId, "CANCELLED");

        ProjectInvitation invitation = ProjectInvitation.Create(projectId, inviteeId, MemberType.User, roleId);
        _invitationRepositoryMock.GetByIdTrackedAsync(projectId, invitationId, Arg.Any<CancellationToken>())
            .Returns(invitation);
        _userContextMock.UserId.Returns(userId);
        _permissionCheckerMock.CheckAsync(
            projectId, userId, "project.invite-members", Arg.Any<CancellationToken>())
            .Returns(ProjectPermissionCheckStatus.Denied);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
    }
}
