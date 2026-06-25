using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.WorkItems;
using Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;
using Modules.WorkItems.Domain.Errors;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.WorkItems.GetDetails;

public class GetWorkItemDetailsQueryHandlerTests
{
    private readonly GetWorkItemDetailsQueryHandler _handler;
    private readonly IGetWorkItemDetailsQueryService _serviceMock;

    public GetWorkItemDetailsQueryHandlerTests()
    {
        _serviceMock = Substitute.For<IGetWorkItemDetailsQueryService>();
        _handler = new GetWorkItemDetailsQueryHandler(_serviceMock);
    }

    private static WorkItemDetailDto BuildDto(Guid workItemId) =>
        new(workItemId, "TST-1", "Title", null, null, null, null,
            new WorkItemDedicationDto(0, 0), 0,
            Guid.NewGuid(), "Task", Guid.NewGuid(), "To Do", null,
            [], [], [], []);

    [Fact]
    public async Task Handle_Should_ReturnDetails_When_WorkItemExists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        WorkItemDetailDto dto = BuildDto(workItemId);
        _serviceMock.GetWorkItemDetailsAsync(projectId, workItemId, Arg.Any<CancellationToken>()).Returns(dto);
        GetWorkItemDetailsQuery query = new(projectId, workItemId);

        // Act
        Result<WorkItemDetailDto> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        _serviceMock.GetWorkItemDetailsAsync(projectId, workItemId, Arg.Any<CancellationToken>()).Returns((WorkItemDetailDto?)null);
        GetWorkItemDetailsQuery query = new(projectId, workItemId);

        // Act
        Result<WorkItemDetailDto> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);
    }
}
