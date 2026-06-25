using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.WorkItems.GetProjectWorkItems;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.WorkItems.GetProjectWorkItems;

public class GetProjectWorkItemsQueryHandlerTests
{
    private readonly GetProjectWorkItemsQueryHandler _handler;
    private readonly IGetProjectWorkItemsQueryService _serviceMock;

    public GetProjectWorkItemsQueryHandlerTests()
    {
        _serviceMock = Substitute.For<IGetProjectWorkItemsQueryService>();
        _handler = new GetProjectWorkItemsQueryHandler(_serviceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnPaginatedWorkItems()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        PaginatedResult<WorkItemListDto> expected = new([], null, false);
        _serviceMock.GetProjectWorkItemsAsync(projectId, Arg.Any<int>(), Arg.Any<string?>(),
            Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>()).Returns(expected);

        GetProjectWorkItemsQuery query = new() { ProjectId = projectId };

        // Act
        Result<PaginatedResult<WorkItemListDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected);
    }
}
