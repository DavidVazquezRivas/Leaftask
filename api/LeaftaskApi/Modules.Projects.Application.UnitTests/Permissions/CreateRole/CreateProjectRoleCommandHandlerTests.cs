using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Permissions.CreateRole;
using Modules.Projects.Application.Permissions.GetRoles;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Permissions.CreateRole;

public class CreateProjectRoleCommandHandlerTests
{
    private readonly CreateProjectRoleCommandHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IProjectRoleRepository _roleRepositoryMock;

    public CreateProjectRoleCommandHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _roleRepositoryMock = Substitute.For<IProjectRoleRepository>();

        _handler = new CreateProjectRoleCommandHandler(
            _projectRepositoryMock,
            _roleRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithRole_When_ValidCommandProvided()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        CreateProjectRoleCommand command = new(projectId, "Developer", []);

        Project project = ProjectTestBuilder.AProject().Build();
        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(project);
        _roleRepositoryMock.ExistsByNameAsync(projectId, "Developer", cancellationToken: Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result<ProjectRoleDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Developer");
        await _roleRepositoryMock.Received(1).AddAsync(Arg.Any<ProjectRole>(), Arg.Any<CancellationToken>());
        await _roleRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        CreateProjectRoleCommand command = new(projectId, "Developer", []);

        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        // Act
        Result<ProjectRoleDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
        await _roleRepositoryMock.DidNotReceive().AddAsync(Arg.Any<ProjectRole>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RoleNameAlreadyExists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        CreateProjectRoleCommand command = new(projectId, "Developer", []);

        Project project = ProjectTestBuilder.AProject().Build();
        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(project);
        _roleRepositoryMock.ExistsByNameAsync(projectId, "Developer", cancellationToken: Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result<ProjectRoleDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.DuplicatedRoleName);
        await _roleRepositoryMock.DidNotReceive().AddAsync(Arg.Any<ProjectRole>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_PermissionNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid permissionId = Guid.NewGuid();
        CreateProjectRoleCommand command = new(
            projectId,
            "Developer",
            [new CreateProjectRolePermissionInput(permissionId, PermissionLevel.Full)]);

        Project project = ProjectTestBuilder.AProject().Build();
        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(project);
        _roleRepositoryMock.ExistsByNameAsync(projectId, "Developer", cancellationToken: Arg.Any<CancellationToken>())
            .Returns(false);
        _roleRepositoryMock.GetPermissionByIdAsync(permissionId, Arg.Any<CancellationToken>())
            .Returns((ProjectPermission?)null);

        // Act
        Result<ProjectRoleDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.PermissionNotFound);
        await _roleRepositoryMock.DidNotReceive().AddAsync(Arg.Any<ProjectRole>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_CreateRoleWithPermissions_When_PermissionsProvided()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid permissionId = Guid.NewGuid();
        CreateProjectRoleCommand command = new(
            projectId,
            "Developer",
            [new CreateProjectRolePermissionInput(permissionId, PermissionLevel.Full)]);

        Project project = ProjectTestBuilder.AProject().Build();
        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(project);
        _roleRepositoryMock.ExistsByNameAsync(projectId, "Developer", cancellationToken: Arg.Any<CancellationToken>())
            .Returns(false);

        ProjectPermissionGroup group = new(Guid.NewGuid(), "Project Management");
        ProjectPermission permission = new(permissionId, "project.view", "View project", group);
        _roleRepositoryMock.GetPermissionByIdAsync(permissionId, Arg.Any<CancellationToken>())
            .Returns(permission);

        // Act
        Result<ProjectRoleDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Permissions.Should().HaveCount(1);
        result.Value.Permissions[0].PermissionId.Should().Be(permissionId);
        await _roleRepositoryMock.Received(1).AddPermissionsAsync(
            Arg.Any<IEnumerable<ProjectRolePermission>>(),
            Arg.Any<CancellationToken>());
    }
}
