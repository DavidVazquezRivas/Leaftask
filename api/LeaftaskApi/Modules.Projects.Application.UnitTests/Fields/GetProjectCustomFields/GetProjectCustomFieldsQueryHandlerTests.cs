using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Fields.GetProjectCustomFields;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Fields.GetProjectCustomFields;

public class GetProjectCustomFieldsQueryHandlerTests
{
    private readonly GetProjectCustomFieldsQueryHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IProjectAccessChecker _accessCheckerMock;
    private readonly IUserContext _userContextMock;
    private readonly IGetProjectCustomFieldsQueryService _queryServiceMock;

    public GetProjectCustomFieldsQueryHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _accessCheckerMock = Substitute.For<IProjectAccessChecker>();
        _userContextMock = Substitute.For<IUserContext>();
        _queryServiceMock = Substitute.For<IGetProjectCustomFieldsQueryService>();

        _handler = new GetProjectCustomFieldsQueryHandler(
            _projectRepositoryMock,
            _accessCheckerMock,
            _userContextMock,
            _queryServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFields_When_ProjectExistsAndUserHasAccess()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        GetProjectCustomFieldsQuery query = new(projectId);

        Project project = ProjectTestBuilder.AProject().Build();
        _userContextMock.UserId.Returns(userId);
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _accessCheckerMock.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(true);

        IReadOnlyList<CustomFieldDto> fields =
        [
            new CustomFieldDto(Guid.NewGuid(), "Priority", Guid.NewGuid(), [], true, [])
        ];
        _queryServiceMock.GetCustomFieldsAsync(projectId, Arg.Any<CancellationToken>()).Returns(fields);

        // Act
        Result<IReadOnlyList<CustomFieldDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        GetProjectCustomFieldsQuery query = new(projectId);

        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        // Act
        Result<IReadOnlyList<CustomFieldDto>> result = await _handler.Handle(query, CancellationToken.None);

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
        GetProjectCustomFieldsQuery query = new(projectId);

        Project project = ProjectTestBuilder.AProject().Build();
        _userContextMock.UserId.Returns(userId);
        _projectRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);
        _accessCheckerMock.CanAccessAsync(project, userId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result<IReadOnlyList<CustomFieldDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.AccessDenied);
        await _queryServiceMock.DidNotReceive().GetCustomFieldsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
