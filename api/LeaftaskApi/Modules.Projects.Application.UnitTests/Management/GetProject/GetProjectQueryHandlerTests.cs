using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Management.Create;
using Modules.Projects.Application.Management.GetProject;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Management.GetProject;

public class GetProjectQueryHandlerTests
{
    private readonly GetProjectQueryHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IOrganizationPermissionChecker _permissionCheckerMock;
    private readonly IUserContext _userContextMock;

    public GetProjectQueryHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _permissionCheckerMock = Substitute.For<IOrganizationPermissionChecker>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new GetProjectQueryHandler(
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

        Project project = ProjectTestBuilder.AProject().OwnedByUser(userId).Build();

        _projectRepositoryMock.GetByIdAsync(project.Id, Arg.Any<CancellationToken>())
            .Returns(project);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(new GetProjectQuery(project.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(project.Id);
        result.Value.Name.Should().Be(project.Name);
        result.Value.OrganizationId.Should().BeNull();

        await _permissionCheckerMock.DidNotReceive()
            .IsMemberAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_OrganizationProjectAndUserIsMember()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid organizationId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        Project project = ProjectTestBuilder.AProject().OwnedByOrganization(organizationId).Build();

        _projectRepositoryMock.GetByIdAsync(project.Id, Arg.Any<CancellationToken>())
            .Returns(project);

        _permissionCheckerMock.IsMemberAsync(organizationId, userId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(new GetProjectQuery(project.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(project.Id);
        result.Value.OrganizationId.Should().Be(organizationId);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectDoesNotExist()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        _userContextMock.UserId.Returns(Guid.NewGuid());

        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(new GetProjectQuery(projectId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_PersonalProjectAndUserIsNotOwner()
    {
        // Arrange
        _userContextMock.UserId.Returns(Guid.NewGuid());

        Project project = ProjectTestBuilder.AProject().OwnedByUser(Guid.NewGuid()).Build();

        _projectRepositoryMock.GetByIdAsync(project.Id, Arg.Any<CancellationToken>())
            .Returns(project);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(new GetProjectQuery(project.Id), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OrganizationProjectAndUserIsNotMember()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid organizationId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        Project project = ProjectTestBuilder.AProject().OwnedByOrganization(organizationId).Build();

        _projectRepositoryMock.GetByIdAsync(project.Id, Arg.Any<CancellationToken>())
            .Returns(project);

        _permissionCheckerMock.IsMemberAsync(organizationId, userId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(new GetProjectQuery(project.Id), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
    }
}
