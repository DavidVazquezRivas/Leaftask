using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Management.GetMyProjects;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Management.GetMyProjects;

public class GetMyProjectsQueryHandlerTests
{
    private readonly GetMyProjectsQueryHandler _handler;
    private readonly IGetMyProjectsQueryService _queryServiceMock;
    private readonly IUserContext _userContextMock;

    public GetMyProjectsQueryHandlerTests()
    {
        _queryServiceMock = Substitute.For<IGetMyProjectsQueryService>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new GetMyProjectsQueryHandler(_queryServiceMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_ProjectsFromService()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        SimpleProjectDto[] projects =
        [
            new(Guid.NewGuid(), "Project A", "PRJ"),
            new(Guid.NewGuid(), "Project B", "PRB")
        ];
        PaginatedResult<SimpleProjectDto> paginatedResult = new(projects, null, false);

        GetMyProjectsQuery query = new() { Limit = 10, Cursor = null, Sort = [] };

        _queryServiceMock.GetMyProjectsAsync(userId, query.Limit, query.Cursor, query.Sort, Arg.Any<CancellationToken>())
            .Returns(paginatedResult);

        // Act
        Result<PaginatedResult<SimpleProjectDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.HasMore.Should().BeFalse();
    }
}
