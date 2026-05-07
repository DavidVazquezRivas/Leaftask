using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Management.Create;
using Modules.Projects.Application.Management.Patch;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Management.Patch;

public class PatchProjectCommandHandlerTests
{
    private readonly PatchProjectCommandHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IOrganizationPermissionChecker _permissionCheckerMock;
    private readonly IUserContext _userContextMock;

    public PatchProjectCommandHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _permissionCheckerMock = Substitute.For<IOrganizationPermissionChecker>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new PatchProjectCommandHandler(
            _projectRepositoryMock,
            _permissionCheckerMock,
            _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_PersonalProjectAndUserIsOwner()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        Project project = ProjectTestBuilder.AProject()
            .WithName("Old Name")
            .OwnedByUser(userId)
            .Build();

        _projectRepositoryMock.GetByIdTrackedAsync(project.Id, Arg.Any<CancellationToken>())
            .Returns(project);

        PatchProjectCommand command = new(project.Id, "New Name", null, null);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("New Name");
        await _projectRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_OrganizationProjectAndUserIsMember()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid organizationId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        Project project = ProjectTestBuilder.AProject()
            .OwnedByOrganization(organizationId)
            .Build();

        _projectRepositoryMock.GetByIdTrackedAsync(project.Id, Arg.Any<CancellationToken>())
            .Returns(project);

        _permissionCheckerMock.IsMemberAsync(organizationId, userId, Arg.Any<CancellationToken>())
            .Returns(true);

        PatchProjectCommand command = new(project.Id, null, null, ProjectPrivacy.Private);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PrivacyLevel.Should().Be(ProjectPrivacy.Private);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectDoesNotExist()
    {
        // Arrange
        _userContextMock.UserId.Returns(Guid.NewGuid());

        _projectRepositoryMock.GetByIdTrackedAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        PatchProjectCommand command = new(Guid.NewGuid(), "New Name", null, null);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
        await _projectRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserHasNoAccess()
    {
        // Arrange
        _userContextMock.UserId.Returns(Guid.NewGuid());

        Project project = ProjectTestBuilder.AProject().OwnedByUser(Guid.NewGuid()).Build();

        _projectRepositoryMock.GetByIdTrackedAsync(project.Id, Arg.Any<CancellationToken>())
            .Returns(project);

        PatchProjectCommand command = new(project.Id, "New Name", null, null);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
        await _projectRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AbbreviationIsTaken()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        Project project = ProjectTestBuilder.AProject()
            .WithAbbreviation("OLD")
            .OwnedByUser(userId)
            .Build();

        _projectRepositoryMock.GetByIdTrackedAsync(project.Id, Arg.Any<CancellationToken>())
            .Returns(project);

        _projectRepositoryMock.ExistsByAbbreviationAsync("NEW", userId, project.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        PatchProjectCommand command = new(project.Id, null, "NEW", null);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.DuplicatedAbbreviation);
        await _projectRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
