using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.WorkLogs;
using Modules.WorkItems.Application.WorkLogs.List;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.WorkLogs.List;

public class GetWorkLogsQueryHandlerTests
{
    private readonly GetWorkLogsQueryHandler _handler;
    private readonly IGetWorkLogsQueryService _queryServiceMock;
    private readonly IWorkItemRepository _workItemRepositoryMock;

    public GetWorkLogsQueryHandlerTests()
    {
        _queryServiceMock = Substitute.For<IGetWorkLogsQueryService>();
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();

        _handler = new GetWorkLogsQueryHandler(_queryServiceMock, _workItemRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnPaginatedLogs_When_WorkItemExists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        GetWorkLogsQuery query = new(projectId, workItemId, 10, null, []);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);

        WorkLogUserDto user = new(Guid.NewGuid(), "Alice", "Smith");
        List<WorkLogDto> logs =
        [
            new(Guid.NewGuid(), 2f, DateOnly.FromDateTime(DateTime.Today), user, "Task work"),
            new(Guid.NewGuid(), 3.5f, DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), user, "More work")
        ];
        PaginatedResult<WorkLogDto> paginatedResult = new(logs, null, false);

        _queryServiceMock.GetWorkLogsAsync(projectId, workItemId, 10, null, Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>())
            .Returns(paginatedResult);

        // Act
        Result<PaginatedResult<WorkLogDto>> result = await _handler.Handle(query, CancellationToken.None);

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
        GetWorkLogsQuery query = new(projectId, workItemId, 10, null, []);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result<PaginatedResult<WorkLogDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);

        await _queryServiceMock.DidNotReceive().GetWorkLogsAsync(
            Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<string>(),
            Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>());
    }
}
