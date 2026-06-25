using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Invitations;
using Modules.Organizations.Application.Invitations.Respond;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Invitations.Respond;

public class RespondOrganizationInvitationCommandHandlerTests
{
    private readonly RespondOrganizationInvitationCommandHandler _handler;
    private readonly IOrganizationRepository _repositoryMock;
    private readonly IOrganizationPermissionRepository _permissionRepositoryMock;
    private readonly IUserContext _userContextMock;

    public RespondOrganizationInvitationCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _permissionRepositoryMock = Substitute.For<IOrganizationPermissionRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new RespondOrganizationInvitationCommandHandler(_repositoryMock, _permissionRepositoryMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_InvitedUserAccepts()
    {
        // Arrange
        Guid creatorId = Guid.NewGuid();
        Guid invitedUserId = Guid.NewGuid();
        Organization organization = OrganizationTestBuilder.AnOrganization().WithCreatorUserId(creatorId).Build();
        OrganizationInvitation invitation = organization.AddInvitation(invitedUserId, organization.Roles.First().Id);
        organization.ClearDomainEvents();

        _userContextMock.UserId.Returns(invitedUserId);
        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        RespondOrganizationInvitationCommand command = new(organization.Id, invitation.Id, "accepted");

        // Act
        Result<OrganizationInvitationResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Accepted");
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_InvitedUserRejects()
    {
        // Arrange
        Guid creatorId = Guid.NewGuid();
        Guid invitedUserId = Guid.NewGuid();
        Organization organization = OrganizationTestBuilder.AnOrganization().WithCreatorUserId(creatorId).Build();
        OrganizationInvitation invitation = organization.AddInvitation(invitedUserId, organization.Roles.First().Id);
        organization.ClearDomainEvents();

        _userContextMock.UserId.Returns(invitedUserId);
        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        RespondOrganizationInvitationCommand command = new(organization.Id, invitation.Id, "rejected");

        // Act
        Result<OrganizationInvitationResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Rejected");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OrganizationNotFound()
    {
        // Arrange
        Guid orgId = Guid.NewGuid();
        _repositoryMock.GetByIdAsync(orgId, Arg.Any<CancellationToken>()).Returns((Organization?)null);
        RespondOrganizationInvitationCommand command = new(orgId, Guid.NewGuid(), "accepted");

        // Act
        Result<OrganizationInvitationResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_InvitationNotFound()
    {
        // Arrange
        Guid creatorId = Guid.NewGuid();
        Organization organization = OrganizationTestBuilder.AnOrganization().WithCreatorUserId(creatorId).Build();

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        RespondOrganizationInvitationCommand command = new(organization.Id, Guid.NewGuid(), "accepted");

        // Act
        Result<OrganizationInvitationResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationInvitationNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WrongUser_Tries_To_Accept()
    {
        Guid creatorId = Guid.NewGuid();
        Guid invitedUserId = Guid.NewGuid();
        Guid otherUserId = Guid.NewGuid();
        Organization organization = OrganizationTestBuilder.AnOrganization().WithCreatorUserId(creatorId).Build();
        OrganizationInvitation invitation = organization.AddInvitation(invitedUserId, organization.Roles.First().Id);

        _userContextMock.UserId.Returns(otherUserId);
        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        Result<OrganizationInvitationResponse> result = await _handler.Handle(
            new(organization.Id, invitation.Id, "accepted"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationPermissionDenied);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_InvitationIsNotPending()
    {
        Guid creatorId = Guid.NewGuid();
        Guid invitedUserId = Guid.NewGuid();
        Organization organization = OrganizationTestBuilder.AnOrganization().WithCreatorUserId(creatorId).Build();
        OrganizationInvitation invitation = organization.AddInvitation(invitedUserId, organization.Roles.First().Id);
        invitation.Reject();

        _userContextMock.UserId.Returns(invitedUserId);
        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        Result<OrganizationInvitationResponse> result = await _handler.Handle(
            new(organization.Id, invitation.Id, "accepted"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.InvalidInvitationStatus);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Cancel_And_InviteMembersPermissionNotFound()
    {
        Guid creatorId = Guid.NewGuid();
        Guid invitedUserId = Guid.NewGuid();
        Organization organization = OrganizationTestBuilder.AnOrganization().WithCreatorUserId(creatorId).Build();
        OrganizationInvitation invitation = organization.AddInvitation(invitedUserId, organization.Roles.First().Id);

        _userContextMock.UserId.Returns(creatorId);
        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns([]);

        Result<OrganizationInvitationResponse> result = await _handler.Handle(
            new(organization.Id, invitation.Id, "canceled"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationPermissionNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Cancel_And_UserHasNoInvitePermission()
    {
        Guid creatorId = Guid.NewGuid();
        Guid invitedUserId = Guid.NewGuid();
        Guid cancelerId = Guid.NewGuid();

        OrganizationPermission invitePerm = new("Invite Members", "desc");
        Organization organization = Organization.Create("Org", "desc", "https://org.com", creatorId, [invitePerm]);
        OrganizationInvitation invitation = organization.AddInvitation(invitedUserId, organization.Roles.First().Id);

        _userContextMock.UserId.Returns(cancelerId);
        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns([invitePerm]);

        Result<OrganizationInvitationResponse> result = await _handler.Handle(
            new(organization.Id, invitation.Id, "canceled"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationPermissionDenied);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Cancel_And_UserHasInvitePermission()
    {
        Guid creatorId = Guid.NewGuid();
        Guid invitedUserId = Guid.NewGuid();

        OrganizationPermission invitePerm = new("Invite Members", "desc");
        Organization organization = Organization.Create("Org", "desc", "https://org.com", creatorId, [invitePerm]);
        OrganizationRole ownerRole = organization.Roles.First();
        ownerRole.SetPermissionLevel(invitePerm.Id, PermissionLevel.Full);

        // Creator already has an accepted invitation (auto-created on org creation)
        OrganizationInvitation creatorInvitation = organization.Invitations
            .FirstOrDefault(inv => inv.UserId == creatorId) ??
            organization.AddInvitation(creatorId, ownerRole.Id);
        creatorInvitation.Accept();

        OrganizationInvitation targetInvitation = organization.AddInvitation(invitedUserId, ownerRole.Id);

        _userContextMock.UserId.Returns(creatorId);
        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns([invitePerm]);

        Result<OrganizationInvitationResponse> result = await _handler.Handle(
            new(organization.Id, targetInvitation.Id, "canceled"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Canceled");
    }
}
