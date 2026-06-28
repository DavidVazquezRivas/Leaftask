using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Permissions.GetRoles;
using Modules.Projects.Application.Permissions.PatchRole;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Permissions.PatchRole;

public class PatchProjectRoleCommandHandlerTests
{
    private readonly PatchProjectRoleCommandHandler _handler;
    private readonly IProjectRoleRepository _roleRepositoryMock;

    public PatchProjectRoleCommandHandlerTests()
    {
        _roleRepositoryMock = Substitute.For<IProjectRoleRepository>();
        _handler = new PatchProjectRoleCommandHandler(_roleRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_NameIsUpdated()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        PatchProjectRoleCommand command = new(projectId, roleId, "NewName", null);

        ProjectRole role = new(roleId, "OldName", ProjectTestBuilder.AProject().Build());
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns(role);
        _roleRepositoryMock.ExistsByNameAsync(projectId, "NewName", roleId, Arg.Any<CancellationToken>())
            .Returns(false);
        _roleRepositoryMock.GetRolePermissionsTrackedAsync(roleId, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result<ProjectRoleDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("NewName");
        await _roleRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoleNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        PatchProjectRoleCommand command = new(projectId, roleId, "NewName", null);

        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns((ProjectRole?)null);

        // Act
        Result<ProjectRoleDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.RoleNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OwnerRolePermissionsAreModified()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        IReadOnlyList<PatchProjectRolePermissionInput> permissions =
            [new PatchProjectRolePermissionInput(Guid.NewGuid(), PermissionLevel.Full)];
        PatchProjectRoleCommand command = new(projectId, roleId, null, permissions);

        ProjectRole ownerRole = new(roleId, "Owner", ProjectTestBuilder.AProject().Build(), isOwnerRole: true);
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns(ownerRole);

        // Act
        Result<ProjectRoleDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.OwnerRoleCannotBeModified);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NameAlreadyExists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        PatchProjectRoleCommand command = new(projectId, roleId, "ExistingName", null);

        ProjectRole role = new(roleId, "OldName", ProjectTestBuilder.AProject().Build());
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns(role);
        _roleRepositoryMock.ExistsByNameAsync(projectId, "ExistingName", roleId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result<ProjectRoleDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.DuplicatedRoleName);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_PermissionNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        Guid permissionId = Guid.NewGuid();
        IReadOnlyList<PatchProjectRolePermissionInput> permissions =
            [new PatchProjectRolePermissionInput(permissionId, PermissionLevel.Full)];
        PatchProjectRoleCommand command = new(projectId, roleId, null, permissions);

        ProjectRole role = new(roleId, "Developer", ProjectTestBuilder.AProject().Build());
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns(role);
        _roleRepositoryMock.GetRolePermissionsTrackedAsync(roleId, Arg.Any<CancellationToken>())
            .Returns([]);
        _roleRepositoryMock.GetPermissionByIdAsync(permissionId, Arg.Any<CancellationToken>())
            .Returns((ProjectPermission?)null);

        // Act
        Result<ProjectRoleDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.PermissionNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithNoChanges_When_SameName()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        PatchProjectRoleCommand command = new(projectId, roleId, "SameName", null);

        ProjectRole role = new(roleId, "SameName", ProjectTestBuilder.AProject().Build());
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns(role);
        _roleRepositoryMock.GetRolePermissionsTrackedAsync(roleId, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result<ProjectRoleDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _roleRepositoryMock.DidNotReceive().ExistsByNameAsync(
            Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
