using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.Configuration.GetWorkItemStatuses;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Configuration.GetWorkItemStatuses;

public class GetWorkItemStatusesQueryHandlerTests
{
    private readonly GetWorkItemStatusesQueryHandler _handler;
    private readonly IGetWorkItemStatusesQueryService _queryServiceMock;

    public GetWorkItemStatusesQueryHandlerTests()
    {
        _queryServiceMock = Substitute.For<IGetWorkItemStatusesQueryService>();
        _handler = new GetWorkItemStatusesQueryHandler(_queryServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_WorkItemStatuses()
    {
        // Arrange
        List<WorkItemStatusDto> statuses =
        [
            new WorkItemStatusDto(Guid.NewGuid(), "Por hacer"),
            new WorkItemStatusDto(Guid.NewGuid(), "En progreso"),
            new WorkItemStatusDto(Guid.NewGuid(), "Hecho"),
            new WorkItemStatusDto(Guid.NewGuid(), "Cancelado")
        ];

        _queryServiceMock.GetWorkItemStatusesAsync(Arg.Any<CancellationToken>())
            .Returns(statuses);

        // Act
        Result<IReadOnlyList<WorkItemStatusDto>> result =
            await _handler.Handle(new GetWorkItemStatusesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(statuses);
        await _queryServiceMock.Received(1).GetWorkItemStatusesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_EmptyList_When_NoStatusesExist()
    {
        // Arrange
        _queryServiceMock.GetWorkItemStatusesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<WorkItemStatusDto>());

        // Act
        Result<IReadOnlyList<WorkItemStatusDto>> result =
            await _handler.Handle(new GetWorkItemStatusesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
