using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Roles.Create;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Roles.Create;

public class CreateOrganizationRoleCommandHandlerTests
{
    private readonly CreateOrganizationRoleCommandHandler _handler;
    private readonly IOrganizationPermissionRepository _permissionRepositoryMock;
    private readonly IGetOrganizationRoleDetailsQueryService _queryServiceMock;
    private readonly IOrganizationRepository _repositoryMock;
    private readonly IUserContext _userContextMock;

    public CreateOrganizationRoleCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _permissionRepositoryMock = Substitute.For<IOrganizationPermissionRepository>();
        _queryServiceMock = Substitute.For<IGetOrganizationRoleDetailsQueryService>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new CreateOrganizationRoleCommandHandler(
            _repositoryMock,
            _permissionRepositoryMock,
            _queryServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RoleIsCreated_And_UserHasConfigurePermission()
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

        CreateOrganizationRoleCommand command = new(
            organization.Id,
            "Leaftask Admin",
            [
                new CreateOrganizationRolePermissionInput(configurePermission.Id, PermissionLevel.Full),
                new CreateOrganizationRolePermissionInput(inviteMembersPermission.Id, PermissionLevel.Supervised)
            ]);

        OrganizationRole? addedRole = null;

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);
        _repositoryMock.AddRoleAsync(Arg.Do<OrganizationRole>(role => addedRole = role), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _queryServiceMock.GetOrganizationRoleAsync(organization.Id, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(call => new OrganizationRoleResponse(
                call.ArgAt<Guid>(1),
                command.Name,
                1,
                [
                    new CreateOrganizationRolePermissionResponse(configurePermission.Id, PermissionLevel.Full),
                    new CreateOrganizationRolePermissionResponse(inviteMembersPermission.Id, PermissionLevel.Supervised)
                ]));

        // Act
        Result<OrganizationRoleResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(command.Name);
        result.Value.TotalMembers.Should().Be(1);
        result.Value.Permissions.Should().HaveCount(2);

        addedRole.Should().NotBeNull();
        addedRole!.OrganizationId.Should().Be(organization.Id);
        addedRole.Name.Should().Be(command.Name);
        addedRole.Permissions.Should().ContainSingle(permission =>
            permission.OrganizationPermissionId == configurePermission.Id && permission.Level == PermissionLevel.Full);
        addedRole.Permissions.Should().ContainSingle(permission =>
            permission.OrganizationPermissionId == inviteMembersPermission.Id &&
            permission.Level == PermissionLevel.Supervised);

        await _repositoryMock.Received(1).AddRoleAsync(Arg.Any<OrganizationRole>(), Arg.Any<CancellationToken>());
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _permissionRepositoryMock.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        await _queryServiceMock.Received(1)
            .GetOrganizationRoleAsync(organization.Id, Arg.Any<Guid>(), Arg.Any<CancellationToken>());
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

        CreateOrganizationRoleCommand command = new(
            organization.Id,
            "Leaftask Admin",
            [new CreateOrganizationRolePermissionInput(configurePermission.Id, PermissionLevel.Full)]);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);

        // Act
        Result<OrganizationRoleResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationPermissionDenied);

        await _repositoryMock.DidNotReceive().AddRoleAsync(Arg.Any<OrganizationRole>(), Arg.Any<CancellationToken>());
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
        await _queryServiceMock.DidNotReceive()
            .GetOrganizationRoleAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_PermissionDoesNotExist()
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

        CreateOrganizationRoleCommand command = new(
            organization.Id,
            "Leaftask Admin",
            [new CreateOrganizationRolePermissionInput(Guid.NewGuid(), PermissionLevel.Full)]);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);

        // Act
        Result<OrganizationRoleResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationPermissionNotFound);

        await _repositoryMock.DidNotReceive().AddRoleAsync(Arg.Any<OrganizationRole>(), Arg.Any<CancellationToken>());
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
