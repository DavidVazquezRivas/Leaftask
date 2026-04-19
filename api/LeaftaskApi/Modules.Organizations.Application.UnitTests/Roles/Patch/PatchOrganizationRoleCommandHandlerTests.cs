using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Roles.Create;
using Modules.Organizations.Application.Roles.Patch;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Roles.Patch;

public class PatchOrganizationRoleCommandHandlerTests
{
    private readonly PatchOrganizationRoleCommandHandler _handler;
    private readonly IOrganizationPermissionRepository _permissionRepositoryMock;
    private readonly IGetOrganizationRoleDetailsQueryService _queryServiceMock;
    private readonly IOrganizationRepository _repositoryMock;
    private readonly IUserContext _userContextMock;

    public PatchOrganizationRoleCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _permissionRepositoryMock = Substitute.For<IOrganizationPermissionRepository>();
        _queryServiceMock = Substitute.For<IGetOrganizationRoleDetailsQueryService>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new PatchOrganizationRoleCommandHandler(
            _repositoryMock,
            _permissionRepositoryMock,
            _queryServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_UserIsMember_And_HasConfigurePermission()
    {
        // Arrange
        Guid creatorUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(creatorUserId);

        OrganizationPermission configurePermission = new("Configure Organization",
            "Modify organization settings, branding, and general configuration");
        OrganizationPermission inviteMembersPermission =
            new("Invite Members", "Send invitations to new members to join the organization");
        OrganizationPermission[] permissions = [configurePermission, inviteMembersPermission];

        Organization organization = Organization.Create(
            "Leaftask",
            "Organization description",
            "https://leaftask.com",
            creatorUserId,
            permissions);

        OrganizationRole role = organization.AddRole("Leaftask Admin", permissions);
        role.SetPermissionLevel(configurePermission.Id, PermissionLevel.Full);
        role.SetPermissionLevel(inviteMembersPermission.Id, PermissionLevel.Supervised);

        PatchOrganizationRoleCommand command = new(
            organization.Id,
            role.Id,
            "Leaftask Admin Updated",
            [new PatchOrganizationRolePermissionInput(configurePermission.Id, PermissionLevel.Full)]);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);
        _queryServiceMock.GetOrganizationRoleAsync(organization.Id, role.Id, Arg.Any<CancellationToken>())
            .Returns(new OrganizationRoleResponse(
                role.Id,
                "Leaftask Admin Updated",
                1,
                [new CreateOrganizationRolePermissionResponse(configurePermission.Id, PermissionLevel.Full)]));

        // Act
        Result<OrganizationRoleResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Leaftask Admin Updated");
        result.Value.Permissions.Should().ContainSingle(permission =>
            permission.Id == configurePermission.Id && permission.Level == PermissionLevel.Full);

        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _permissionRepositoryMock.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        await _queryServiceMock.Received(1)
            .GetOrganizationRoleAsync(organization.Id, role.Id, Arg.Any<CancellationToken>());
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

        OrganizationRole role = organization.AddRole("Leaftask Admin", permissions);
        role.SetPermissionLevel(configurePermission.Id, PermissionLevel.Full);

        PatchOrganizationRoleCommand command = new(
            organization.Id,
            role.Id,
            "Leaftask Admin Updated",
            [new PatchOrganizationRolePermissionInput(configurePermission.Id, PermissionLevel.Full)]);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);

        // Act
        Result<OrganizationRoleResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationPermissionDenied);
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoleDoesNotExist()
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

        PatchOrganizationRoleCommand command = new(
            organization.Id,
            Guid.NewGuid(),
            "Leaftask Admin Updated",
            [new PatchOrganizationRolePermissionInput(configurePermission.Id, PermissionLevel.Full)]);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);

        // Act
        Result<OrganizationRoleResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationRoleNotFound);
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
