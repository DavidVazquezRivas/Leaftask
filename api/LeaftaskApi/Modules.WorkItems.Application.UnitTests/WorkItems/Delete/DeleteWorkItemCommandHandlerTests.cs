using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.WorkItems.Delete;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.WorkItems.Delete;

public class DeleteWorkItemCommandHandlerTests
{
    private readonly DeleteWorkItemCommandHandler _handler;
    private readonly IWorkItemRepository _repositoryMock;

    public DeleteWorkItemCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IWorkItemRepository>();
        _handler = new DeleteWorkItemCommandHandler(_repositoryMock);
    }

    private static WorkItem CreateWorkItem(Guid projectId)
    {
        ProjectReadModel project = new(projectId, "TST");
        WorkItemStatus status = new(Guid.NewGuid(), "To Do");
        WorkItemType type = new(Guid.NewGuid(), "Task");
        return WorkItem.Create(1, "Title", "Desc", 3m, DateTime.UtcNow.AddDays(7), project, status, type);
    }

    [Fact]
    public async Task Handle_Should_RemoveWorkItem_And_ReturnSuccess()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        WorkItem workItem = CreateWorkItem(projectId);

        _repositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>()).Returns(workItem);
        DeleteWorkItemCommand command = new(projectId, workItemId);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Received(1).Remove(workItem);
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid workItemId = Guid.NewGuid();
        Guid projectId = Guid.NewGuid();
        _repositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>()).Returns((WorkItem?)null);
        DeleteWorkItemCommand command = new(projectId, workItemId);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);
    }
}
