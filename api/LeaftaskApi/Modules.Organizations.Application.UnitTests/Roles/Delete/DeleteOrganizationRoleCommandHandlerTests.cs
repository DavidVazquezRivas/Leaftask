using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Roles.Delete;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Roles.Delete;

public class DeleteOrganizationRoleCommandHandlerTests
{
    private readonly DeleteOrganizationRoleCommandHandler _handler;
    private readonly IOrganizationPermissionRepository _permissionRepositoryMock;
    private readonly IOrganizationRepository _repositoryMock;
    private readonly IUserContext _userContextMock;

    public DeleteOrganizationRoleCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _permissionRepositoryMock = Substitute.For<IOrganizationPermissionRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new DeleteOrganizationRoleCommandHandler(_repositoryMock, _permissionRepositoryMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_UserIsMember_And_HasConfigurePermission()
    {
        // Arrange
        Guid creatorUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(creatorUserId);

        OrganizationPermission configurePermission = new("Configure Organization", "Modify organization settings, branding, and general configuration");
        OrganizationPermission[] permissions = [configurePermission];

        Organization organization = Organization.Create(
            "Leaftask",
            "Organization description",
            "https://leaftask.com",
            creatorUserId,
            permissions);

        OrganizationRole role = organization.AddRole("Leaftask Admin", permissions);
        role.SetPermissionLevel(configurePermission.Id, PermissionLevel.Full);

        DeleteOrganizationRoleCommand command = new(organization.Id, role.Id);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        organization.Roles.Should().NotContain(role);
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserDoesNotHaveConfigurePermission()
    {
        // Arrange
        Guid creatorUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(Guid.NewGuid());

        OrganizationPermission configurePermission = new("Configure Organization", "Modify organization settings, branding, and general configuration");
        OrganizationPermission[] permissions = [configurePermission];

        Organization organization = Organization.Create(
            "Leaftask",
            "Organization description",
            "https://leaftask.com",
            creatorUserId,
            permissions);

        OrganizationRole role = organization.AddRole("Leaftask Admin", permissions);
        role.SetPermissionLevel(configurePermission.Id, PermissionLevel.Full);

        DeleteOrganizationRoleCommand command = new(organization.Id, role.Id);

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
    public async Task Handle_Should_ReturnFailure_When_RoleDoesNotExist()
    {
        // Arrange
        Guid creatorUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(creatorUserId);

        OrganizationPermission configurePermission = new("Configure Organization", "Modify organization settings, branding, and general configuration");
        OrganizationPermission[] permissions = [configurePermission];

        Organization organization = Organization.Create(
            "Leaftask",
            "Organization description",
            "https://leaftask.com",
            creatorUserId,
            permissions);

        DeleteOrganizationRoleCommand command = new(organization.Id, Guid.NewGuid());

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _permissionRepositoryMock.GetAllAsync(Arg.Any<CancellationToken>()).Returns(permissions);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationRoleNotFound);
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
