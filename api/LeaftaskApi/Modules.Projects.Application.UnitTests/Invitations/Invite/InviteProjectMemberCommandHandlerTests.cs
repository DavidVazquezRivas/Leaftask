using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Invitations.Invite;
using Modules.Projects.Domain.Entities.Invitation;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Invitations.Invite;

public class InviteProjectMemberCommandHandlerTests
{
    private readonly InviteProjectMemberCommandHandler _handler;
    private readonly IProjectRoleRepository _roleRepositoryMock;
    private readonly IProjectMemberRepository _memberRepositoryMock;
    private readonly IProjectInvitationRepository _invitationRepositoryMock;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;

    public InviteProjectMemberCommandHandlerTests()
    {
        _roleRepositoryMock = Substitute.For<IProjectRoleRepository>();
        _memberRepositoryMock = Substitute.For<IProjectMemberRepository>();
        _invitationRepositoryMock = Substitute.For<IProjectInvitationRepository>();
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();

        _handler = new InviteProjectMemberCommandHandler(
            _roleRepositoryMock,
            _memberRepositoryMock,
            _invitationRepositoryMock,
            _userReadModelRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_CreateInvitation_When_ValidCommandProvided()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        InviteProjectMemberCommand command = new(projectId, userId, roleId);

        _userReadModelRepositoryMock.ExistsByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(true);

        ProjectRole role = new(roleId, "Member", ProjectTestBuilder.AProject().Build());
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>()).Returns(role);

        _memberRepositoryMock.ExistsByMemberIdAsync(projectId, userId, Arg.Any<CancellationToken>()).Returns(false);
        _invitationRepositoryMock.GetByInviteeTrackedAsync(projectId, userId, Arg.Any<CancellationToken>())
            .Returns((ProjectInvitation?)null);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _invitationRepositoryMock.Received(1).AddAsync(Arg.Any<ProjectInvitation>(), Arg.Any<CancellationToken>());
        await _invitationRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserDoesNotExist()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        InviteProjectMemberCommand command = new(projectId, userId, roleId);

        _userReadModelRepositoryMock.ExistsByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.OwnerNotFound);
        await _invitationRepositoryMock.DidNotReceive().AddAsync(Arg.Any<ProjectInvitation>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoleNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        InviteProjectMemberCommand command = new(projectId, userId, roleId);

        _userReadModelRepositoryMock.ExistsByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(true);
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns((ProjectRole?)null);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.RoleNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserAlreadyMember()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        InviteProjectMemberCommand command = new(projectId, userId, roleId);

        _userReadModelRepositoryMock.ExistsByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(true);

        ProjectRole role = new(roleId, "Member", ProjectTestBuilder.AProject().Build());
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>()).Returns(role);

        _memberRepositoryMock.ExistsByMemberIdAsync(projectId, userId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.UserAlreadyMember);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserAlreadyHasPendingInvitation()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        InviteProjectMemberCommand command = new(projectId, userId, roleId);

        _userReadModelRepositoryMock.ExistsByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(true);

        ProjectRole role = new(roleId, "Member", ProjectTestBuilder.AProject().Build());
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>()).Returns(role);

        _memberRepositoryMock.ExistsByMemberIdAsync(projectId, userId, Arg.Any<CancellationToken>()).Returns(false);

        ProjectInvitation existingInvitation = ProjectInvitation.Create(projectId, userId, MemberType.User, roleId);
        _invitationRepositoryMock.GetByInviteeTrackedAsync(projectId, userId, Arg.Any<CancellationToken>())
            .Returns(existingInvitation);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.UserAlreadyInvited);
    }

    [Fact]
    public async Task Handle_Should_ReactivateInvitation_When_ExistingNonPendingInvitationFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        InviteProjectMemberCommand command = new(projectId, userId, roleId);

        _userReadModelRepositoryMock.ExistsByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(true);

        ProjectRole role = new(roleId, "Member", ProjectTestBuilder.AProject().Build());
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>()).Returns(role);

        _memberRepositoryMock.ExistsByMemberIdAsync(projectId, userId, Arg.Any<CancellationToken>()).Returns(false);

        ProjectInvitation existingInvitation = ProjectInvitation.Create(projectId, userId, MemberType.User, roleId);
        existingInvitation.Reject();
        _invitationRepositoryMock.GetByInviteeTrackedAsync(projectId, userId, Arg.Any<CancellationToken>())
            .Returns(existingInvitation);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(existingInvitation.Id);
        await _invitationRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _invitationRepositoryMock.DidNotReceive().AddAsync(Arg.Any<ProjectInvitation>(), Arg.Any<CancellationToken>());
    }
}
