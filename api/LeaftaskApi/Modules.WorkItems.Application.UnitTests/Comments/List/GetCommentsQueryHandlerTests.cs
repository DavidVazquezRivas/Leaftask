using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.Comments;
using Modules.WorkItems.Application.Comments.List;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Comments.List;

public class GetCommentsQueryHandlerTests
{
    private readonly GetCommentsQueryHandler _handler;
    private readonly IGetCommentsQueryService _queryServiceMock;
    private readonly IWorkItemRepository _workItemRepositoryMock;

    public GetCommentsQueryHandlerTests()
    {
        _queryServiceMock = Substitute.For<IGetCommentsQueryService>();
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();

        _handler = new GetCommentsQueryHandler(_queryServiceMock, _workItemRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnPaginatedComments_When_WorkItemExists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        GetCommentsQuery query = new(projectId, workItemId, 10, null, []);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);

        CommentAuthorDto author = new(Guid.NewGuid(), "Alice Smith");
        List<CommentDto> comments =
        [
            new(Guid.NewGuid(), author, "First comment", DateTime.UtcNow, []),
            new(Guid.NewGuid(), author, "Second comment", DateTime.UtcNow.AddMinutes(1), [])
        ];
        PaginatedResult<CommentDto> paginatedResult = new(comments, null, false);

        _queryServiceMock
            .GetCommentsAsync(projectId, workItemId, 10, null, Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>())
            .Returns(paginatedResult);

        // Act
        Result<PaginatedResult<CommentDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.HasMore.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        GetCommentsQuery query = new(projectId, workItemId, 10, null, []);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result<PaginatedResult<CommentDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);
        await _queryServiceMock.DidNotReceive().GetCommentsAsync(
            Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<string>(),
            Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>());
    }
}
