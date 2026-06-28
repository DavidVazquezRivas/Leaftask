using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Members.GetCount;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Members.GetCount;

public class GetProjectMemberCountQueryHandlerTests
{
    private readonly GetProjectMemberCountQueryHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IProjectMemberRepository _memberRepositoryMock;
    private readonly IProjectAccessChecker _accessCheckerMock;
    private readonly IUserContext _userContextMock;

    public GetProjectMemberCountQueryHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _memberRepositoryMock = Substitute.For<IProjectMemberRepository>();
        _accessCheckerMock = Substitute.For<IProjectAccessChecker>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new GetProjectMemberCountQueryHandler(
            _projectRepositoryMock,
            _memberRepositoryMock,
            _accessCheckerMock,
            _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnCount_When_ProjectExistsAndUserHasAccess()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        GetProjectMemberCountQuery query = new(projectId);

        Project project = ProjectTestBuilder.AProject().Build();
        _userContextMock.UserId.Returns(userId);
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _accessCheckerMock.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(true);
        _memberRepositoryMock.GetCountByProjectAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((3, 2));

        // Act
        Result<ProjectMemberCountDto> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalMembers.Should().Be(5);
        result.Value.People.Should().Be(3);
        result.Value.Agents.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        GetProjectMemberCountQuery query = new(projectId);

        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        // Act
        Result<ProjectMemberCountDto> result = await _handler.Handle(query, CancellationToken.None);

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
        GetProjectMemberCountQuery query = new(projectId);

        Project project = ProjectTestBuilder.AProject().Build();
        _userContextMock.UserId.Returns(userId);
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _accessCheckerMock.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result<ProjectMemberCountDto> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
        await _memberRepositoryMock.DidNotReceive().GetCountByProjectAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
