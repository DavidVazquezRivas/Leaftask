using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Invitations;
using Modules.Organizations.Application.Invitations.Create;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Events;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Invitations.Create;

public class CreateOrganizationInvitationCommandHandlerTests
{
    private readonly CreateOrganizationInvitationCommandHandler _handler;
    private readonly IOrganizationPermissionRepository _permissionRepositoryMock;
    private readonly IOrganizationRepository _repositoryMock;
    private readonly IUserContext _userContextMock;

    public CreateOrganizationInvitationCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _permissionRepositoryMock = Substitute.For<IOrganizationPermissionRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new CreateOrganizationInvitationCommandHandler(_repositoryMock, _permissionRepositoryMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_UserHasInviteMembersPermission()
    {
        // Arrange
        Guid creatorUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(creatorUserId);

        OrganizationPermission invitePermission = new("Invite Members", "Send invitations to new members to join the organization");
        OrganizationPermission[] permissions = [invitePermission];

        Organization organization = Organization.Create(
            "Leaftask",
            "Organization description",
            "https://leaftask.com",
            creatorUserId,
            permissions);

        OrganizationRole inviteRole = organization.AddRole("Inviter", permissions);
        inviteRole.SetPermissionLevel(invitePermission.Id, PermissionLevel.Full);

        Guid targetUserId = Guid.NewGuid();
        CreateOrganizationInvitationCommand command = new(organization.Id, targetUserId, inviteRole.Id);

        OrganizationInvitation? addedInvitation = null;

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);
        _repositoryMock.AddInvitationAsync(Arg.Do<OrganizationInvitation>(invitation => addedInvitation = invitation), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        Result<OrganizationInvitationResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.OrganizationId.Should().Be(organization.Id);
        result.Value.UserId.Should().Be(targetUserId);
        result.Value.OrganizationRoleId.Should().Be(inviteRole.Id);
        result.Value.Status.Should().Be(InvitationStatus.Pending.ToString());
        organization.Invitations.Should().ContainSingle(invitation => invitation.UserId == creatorUserId && invitation.Status == InvitationStatus.Accepted);
        addedInvitation.Should().NotBeNull();
        addedInvitation!.OrganizationId.Should().Be(organization.Id);
        addedInvitation.UserId.Should().Be(targetUserId);
        addedInvitation.OrganizationRoleId.Should().Be(inviteRole.Id);
        addedInvitation.Status.Should().Be(InvitationStatus.Pending);
        addedInvitation.DomainEvents.Should().ContainSingle(domainEvent => domainEvent is OrganizationInvitationCreatedDomainEvent);
        organization.DomainEvents.Should().Contain(domainEvent => domainEvent is OrganizationCreatedDomainEvent);
        await _repositoryMock.Received(1).AddInvitationAsync(Arg.Any<OrganizationInvitation>(), Arg.Any<CancellationToken>());
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserDoesNotHaveInviteMembersPermission()
    {
        // Arrange
        Guid creatorUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(Guid.NewGuid());

        OrganizationPermission invitePermission = new("Invite Members", "Send invitations to new members to join the organization");
        OrganizationPermission[] permissions = [invitePermission];

        Organization organization = Organization.Create(
            "Leaftask",
            "Organization description",
            "https://leaftask.com",
            creatorUserId,
            permissions);

        OrganizationRole inviteRole = organization.AddRole("Inviter", permissions);
        inviteRole.SetPermissionLevel(invitePermission.Id, PermissionLevel.Full);

        CreateOrganizationInvitationCommand command = new(organization.Id, Guid.NewGuid(), inviteRole.Id);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);

        // Act
        Result<OrganizationInvitationResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationPermissionDenied);
        await _repositoryMock.DidNotReceive().AddInvitationAsync(Arg.Any<OrganizationInvitation>(), Arg.Any<CancellationToken>());
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoleDoesNotExist()
    {
        // Arrange
        Guid creatorUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(creatorUserId);

        OrganizationPermission invitePermission = new("Invite Members", "Send invitations to new members to join the organization");
        OrganizationPermission[] permissions = [invitePermission];

        Organization organization = Organization.Create(
            "Leaftask",
            "Organization description",
            "https://leaftask.com",
            creatorUserId,
            permissions);

        CreateOrganizationInvitationCommand command = new(organization.Id, Guid.NewGuid(), Guid.NewGuid());

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);

        // Act
        Result<OrganizationInvitationResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationRoleNotFound);
        await _repositoryMock.DidNotReceive().AddInvitationAsync(Arg.Any<OrganizationInvitation>(), Arg.Any<CancellationToken>());
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
