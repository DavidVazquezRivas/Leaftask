using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Permissions.DeleteRole;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Permissions.DeleteRole;

public class DeleteProjectRoleCommandHandlerTests
{
    private readonly DeleteProjectRoleCommandHandler _handler;
    private readonly IProjectRoleRepository _roleRepositoryMock;

    public DeleteProjectRoleCommandHandlerTests()
    {
        _roleRepositoryMock = Substitute.For<IProjectRoleRepository>();
        _handler = new DeleteProjectRoleCommandHandler(_roleRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_RoleIsDeleted()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        DeleteProjectRoleCommand command = new(projectId, roleId);

        ProjectRole role = new(roleId, "Developer", ProjectTestBuilder.AProject().Build());
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns(role);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _roleRepositoryMock.Received(1).Remove(role);
        await _roleRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoleNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        DeleteProjectRoleCommand command = new(projectId, roleId);

        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns((ProjectRole?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.RoleNotFound);
        _roleRepositoryMock.DidNotReceive().Remove(Arg.Any<ProjectRole>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoleIsOwnerRole()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid roleId = Guid.NewGuid();
        DeleteProjectRoleCommand command = new(projectId, roleId);

        ProjectRole ownerRole = new(roleId, "Owner", ProjectTestBuilder.AProject().Build(), isOwnerRole: true);
        _roleRepositoryMock.GetByIdTrackedAsync(projectId, roleId, Arg.Any<CancellationToken>())
            .Returns(ownerRole);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.OwnerRoleCannotBeDeleted);
        _roleRepositoryMock.DidNotReceive().Remove(Arg.Any<ProjectRole>());
    }
}
