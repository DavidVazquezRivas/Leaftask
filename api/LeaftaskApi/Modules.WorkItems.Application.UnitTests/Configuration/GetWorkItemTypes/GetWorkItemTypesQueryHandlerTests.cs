using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.Configuration.GetWorkItemTypes;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Configuration.GetWorkItemTypes;

public class GetWorkItemTypesQueryHandlerTests
{
    private readonly GetWorkItemTypesQueryHandler _handler;
    private readonly IGetWorkItemTypesQueryService _queryServiceMock;

    public GetWorkItemTypesQueryHandlerTests()
    {
        _queryServiceMock = Substitute.For<IGetWorkItemTypesQueryService>();
        _handler = new GetWorkItemTypesQueryHandler(_queryServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_WorkItemTypes()
    {
        // Arrange
        List<WorkItemTypeDto> types =
        [
            new WorkItemTypeDto(Guid.NewGuid(), "Task"),
            new WorkItemTypeDto(Guid.NewGuid(), "Bug")
        ];

        _queryServiceMock.GetWorkItemTypesAsync(Arg.Any<CancellationToken>())
            .Returns(types);

        // Act
        Result<IReadOnlyList<WorkItemTypeDto>> result =
            await _handler.Handle(new GetWorkItemTypesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(types);
        await _queryServiceMock.Received(1).GetWorkItemTypesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_EmptyList_When_NoTypesExist()
    {
        // Arrange
        _queryServiceMock.GetWorkItemTypesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<WorkItemTypeDto>());

        // Act
        Result<IReadOnlyList<WorkItemTypeDto>> result =
            await _handler.Handle(new GetWorkItemTypesQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
