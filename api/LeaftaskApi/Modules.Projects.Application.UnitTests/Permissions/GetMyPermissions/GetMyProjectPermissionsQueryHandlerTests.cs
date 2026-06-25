using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Permissions.GetMyPermissions;
using Modules.Projects.Application.Permissions.GetPermissions;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Permissions.GetMyPermissions;

public class GetMyProjectPermissionsQueryHandlerTests
{
    private readonly GetMyProjectPermissionsQueryHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IProjectMemberRepository _memberRepositoryMock;
    private readonly IProjectRoleRepository _roleRepositoryMock;
    private readonly IOrganizationPermissionChecker _orgPermissionCheckerMock;
    private readonly IGetProjectPermissionsQueryService _allPermissionsServiceMock;
    private readonly IUserContext _userContextMock;

    public GetMyProjectPermissionsQueryHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _memberRepositoryMock = Substitute.For<IProjectMemberRepository>();
        _roleRepositoryMock = Substitute.For<IProjectRoleRepository>();
        _orgPermissionCheckerMock = Substitute.For<IOrganizationPermissionChecker>();
        _allPermissionsServiceMock = Substitute.For<IGetProjectPermissionsQueryService>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new GetMyProjectPermissionsQueryHandler(
            _projectRepositoryMock,
            _memberRepositoryMock,
            _roleRepositoryMock,
            _orgPermissionCheckerMock,
            _allPermissionsServiceMock,
            _userContextMock);
    }

    private static IReadOnlyList<ProjectPermissionDto> BuildPermissions() =>
        [new(Guid.NewGuid(), "project.view", "View", "ReadOnly")];

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectNotFound()
    {
        Guid projectId = Guid.NewGuid();
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns((Project?)null);

        Result<IReadOnlyList<ProjectPermissionLevelDto>> result =
            await _handler.Handle(new(projectId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnAllFull_When_UserOwnsProject()
    {
        Guid userId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Project project = ProjectTestBuilder.AProject().OwnedByUser(userId).Build();

        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _userContextMock.UserId.Returns(userId);
        _allPermissionsServiceMock.GetPermissionsAsync(Arg.Any<CancellationToken>()).Returns(BuildPermissions());

        Result<IReadOnlyList<ProjectPermissionLevelDto>> result =
            await _handler.Handle(new(projectId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().AllSatisfy(p => p.Level.Should().Be("full"));
    }

    [Fact]
    public async Task Handle_Should_ReturnAllFull_When_MemberHasOwnerRole()
    {
        Guid userId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Project project = ProjectTestBuilder.AProject().Build();
        ProjectRole ownerRole = new(Guid.NewGuid(), "Owner", project, isOwnerRole: true);
        ProjectMember member = new(Guid.NewGuid(), userId, MemberType.User, ownerRole, project);

        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _userContextMock.UserId.Returns(userId);
        _allPermissionsServiceMock.GetPermissionsAsync(Arg.Any<CancellationToken>()).Returns(BuildPermissions());
        _memberRepositoryMock.GetByMemberIdTrackedAsync(projectId, userId, Arg.Any<CancellationToken>()).Returns(member);

        Result<IReadOnlyList<ProjectPermissionLevelDto>> result =
            await _handler.Handle(new(projectId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().AllSatisfy(p => p.Level.Should().Be("full"));
    }

    [Fact]
    public async Task Handle_Should_ReturnMappedPermissions_When_MemberHasRegularRole()
    {
        Guid userId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Project project = ProjectTestBuilder.AProject().Build();
        ProjectRole regularRole = new(Guid.NewGuid(), "Member", project, isOwnerRole: false);
        ProjectMember member = new(Guid.NewGuid(), userId, MemberType.User, regularRole, project);

        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _userContextMock.UserId.Returns(userId);
        _allPermissionsServiceMock.GetPermissionsAsync(Arg.Any<CancellationToken>()).Returns(BuildPermissions());
        _memberRepositoryMock.GetByMemberIdTrackedAsync(projectId, userId, Arg.Any<CancellationToken>()).Returns(member);
        _roleRepositoryMock.GetRolePermissionsTrackedAsync(regularRole.Id, Arg.Any<CancellationToken>()).Returns([]);

        Result<IReadOnlyList<ProjectPermissionLevelDto>> result =
            await _handler.Handle(new(projectId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnAllFull_When_NotMemberButIsOrgMember()
    {
        Guid userId = Guid.NewGuid();
        Guid orgId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Project project = ProjectTestBuilder.AProject().OwnedByOrganization(orgId).Build();

        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _userContextMock.UserId.Returns(userId);
        _allPermissionsServiceMock.GetPermissionsAsync(Arg.Any<CancellationToken>()).Returns(BuildPermissions());
        _memberRepositoryMock.GetByMemberIdTrackedAsync(projectId, userId, Arg.Any<CancellationToken>())
            .Returns((ProjectMember?)null);
        _orgPermissionCheckerMock.IsMemberAsync(orgId, userId, Arg.Any<CancellationToken>()).Returns(true);

        Result<IReadOnlyList<ProjectPermissionLevelDto>> result =
            await _handler.Handle(new(projectId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().AllSatisfy(p => p.Level.Should().Be("full"));
    }

    [Fact]
    public async Task Handle_Should_ReturnAccessDenied_When_NotMemberAndNotOrgMember()
    {
        Guid userId = Guid.NewGuid();
        Guid orgId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        Project project = ProjectTestBuilder.AProject().OwnedByOrganization(orgId).Build();

        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _userContextMock.UserId.Returns(userId);
        _allPermissionsServiceMock.GetPermissionsAsync(Arg.Any<CancellationToken>()).Returns(BuildPermissions());
        _memberRepositoryMock.GetByMemberIdTrackedAsync(projectId, userId, Arg.Any<CancellationToken>())
            .Returns((ProjectMember?)null);
        _orgPermissionCheckerMock.IsMemberAsync(orgId, userId, Arg.Any<CancellationToken>()).Returns(false);

        Result<IReadOnlyList<ProjectPermissionLevelDto>> result =
            await _handler.Handle(new(projectId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
    }
}
