using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Members.GetMembers;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Members.GetMembers;

public class GetProjectMembersQueryHandlerTests
{
    private readonly GetProjectMembersQueryHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IProjectAccessChecker _accessCheckerMock;
    private readonly IUserContext _userContextMock;
    private readonly IGetProjectMembersQueryService _queryServiceMock;

    public GetProjectMembersQueryHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _accessCheckerMock = Substitute.For<IProjectAccessChecker>();
        _userContextMock = Substitute.For<IUserContext>();
        _queryServiceMock = Substitute.For<IGetProjectMembersQueryService>();

        _handler = new GetProjectMembersQueryHandler(
            _projectRepositoryMock,
            _accessCheckerMock,
            _userContextMock,
            _queryServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnMembers_When_ProjectExistsAndUserHasAccess()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        GetProjectMembersQuery query = new() { ProjectId = projectId };

        Project project = ProjectTestBuilder.AProject().Build();
        _userContextMock.UserId.Returns(userId);
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _accessCheckerMock.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(true);

        PaginatedResult<ProjectMemberDto> members = new(
            [new ProjectMemberDto(Guid.NewGuid(), "John Doe", "john@example.com", Guid.NewGuid(), "User")],
            null,
            false);
        _queryServiceMock.GetMembersAsync(projectId, query.Limit, query.Cursor, query.Sort, Arg.Any<CancellationToken>())
            .Returns(members);

        // Act
        Result<PaginatedResult<ProjectMemberDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        GetProjectMembersQuery query = new() { ProjectId = projectId };

        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        // Act
        Result<PaginatedResult<ProjectMemberDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserHasNoAccess()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        GetProjectMembersQuery query = new() { ProjectId = projectId };

        Project project = ProjectTestBuilder.AProject().Build();
        _userContextMock.UserId.Returns(userId);
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _accessCheckerMock.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result<PaginatedResult<ProjectMemberDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
        await _queryServiceMock.DidNotReceive().GetMembersAsync(
            Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<string?>(),
            Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>());
    }
}
