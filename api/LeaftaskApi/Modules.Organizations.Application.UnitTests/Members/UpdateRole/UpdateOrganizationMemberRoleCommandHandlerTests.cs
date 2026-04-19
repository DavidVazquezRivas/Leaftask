using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Members.UpdateRole;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Members.UpdateRole;

public class UpdateOrganizationMemberRoleCommandHandlerTests
{
    private readonly UpdateOrganizationMemberRoleCommandHandler _handler;
    private readonly IOrganizationPermissionRepository _permissionRepositoryMock;
    private readonly IOrganizationRepository _repositoryMock;
    private readonly IUserContext _userContextMock;

    public UpdateOrganizationMemberRoleCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _permissionRepositoryMock = Substitute.For<IOrganizationPermissionRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new UpdateOrganizationMemberRoleCommandHandler(_repositoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_UserIsMember_And_HasConfigurePermission()
    {
        // Arrange
        Guid creatorUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(creatorUserId);

        OrganizationPermission configurePermission = new("Configure Organization",
            "Modify organization settings, branding, and general configuration");
        OrganizationPermission[] permissions = [configurePermission];

        Organization organization = Organization.Create(
            "Leaftask",
            "Organization description",
            "https://leaftask.com",
            creatorUserId,
            permissions);

        OrganizationRole adminRole = organization.AddRole("Admin", permissions);
        adminRole.SetPermissionLevel(configurePermission.Id, PermissionLevel.Full);

        OrganizationInvitation targetInvitation = organization.Invitations.Single();
        Guid targetMemberId = targetInvitation.UserId;

        UpdateOrganizationMemberRoleCommand command = new(organization.Id, targetMemberId, adminRole.Id);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        targetInvitation.OrganizationRoleId.Should().Be(adminRole.Id);
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserDoesNotHaveConfigurePermission()
    {
        // Arrange
        Guid creatorUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(Guid.NewGuid());

        OrganizationPermission configurePermission = new("Configure Organization",
            "Modify organization settings, branding, and general configuration");
        OrganizationPermission[] permissions = [configurePermission];

        Organization organization = Organization.Create(
            "Leaftask",
            "Organization description",
            "https://leaftask.com",
            creatorUserId,
            permissions);

        OrganizationRole adminRole = organization.AddRole("Admin", permissions);
        adminRole.SetPermissionLevel(configurePermission.Id, PermissionLevel.Full);

        OrganizationInvitation targetInvitation = organization.Invitations.Single();
        Guid targetMemberId = targetInvitation.UserId;

        UpdateOrganizationMemberRoleCommand command = new(organization.Id, targetMemberId, adminRole.Id);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationPermissionDenied);
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_MemberDoesNotExist()
    {
        // Arrange
        Guid creatorUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(creatorUserId);

        OrganizationPermission configurePermission = new("Configure Organization",
            "Modify organization settings, branding, and general configuration");
        OrganizationPermission[] permissions = [configurePermission];

        Organization organization = Organization.Create(
            "Leaftask",
            "Organization description",
            "https://leaftask.com",
            creatorUserId,
            permissions);

        OrganizationRole adminRole = organization.AddRole("Admin", permissions);
        adminRole.SetPermissionLevel(configurePermission.Id, PermissionLevel.Full);

        UpdateOrganizationMemberRoleCommand command = new(organization.Id, Guid.NewGuid(), adminRole.Id);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationMemberNotFound);
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
