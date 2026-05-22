using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.WorkLogs.Delete;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.WorkLogs.Delete;

public class DeleteWorkLogCommandHandlerTests
{
    private readonly DeleteWorkLogCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepositoryMock;
    private readonly IWorkLogRepository _workLogRepositoryMock;
    private readonly IUserContext _userContextMock;

    public DeleteWorkLogCommandHandlerTests()
    {
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _workLogRepositoryMock = Substitute.For<IWorkLogRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new DeleteWorkLogCommandHandler(
            _workItemRepositoryMock,
            _workLogRepositoryMock,
            _userContextMock);
    }

    private static WorkLog CreateWorkLog(Guid userId)
    {
        ProjectReadModel project = new(Guid.NewGuid(), "TST");
        WorkItemStatus status = new(Guid.NewGuid(), "To Do");
        WorkItemType type = new(Guid.NewGuid(), "Task");
        WorkItem workItem = WorkItem.Create(1, "Title", "Desc", 3m, DateTime.UtcNow.AddDays(7), project, status, type);
        UserReadModel user = new(userId, "John", "Doe");

        return new WorkLog(Guid.NewGuid(), DateTime.UtcNow, 2m, "Work done", workItem, user);
    }

    [Fact]
    public async Task Handle_Should_DeleteWorkLog_When_OwnerDeletes()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid logId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        DeleteWorkLogCommand command = new(projectId, workItemId, logId);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _userContextMock.UserId.Returns(userId);

        WorkLog workLog = CreateWorkLog(userId);
        _workLogRepositoryMock.GetByIdTrackedAsync(logId, workItemId, Arg.Any<CancellationToken>())
            .Returns(workLog);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _workLogRepositoryMock.Received(1).Remove(workLog);
        await _workLogRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        DeleteWorkLogCommand command = new(projectId, workItemId, Guid.NewGuid());

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);
        _workLogRepositoryMock.DidNotReceive().Remove(Arg.Any<WorkLog>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkLogNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid logId = Guid.NewGuid();
        DeleteWorkLogCommand command = new(projectId, workItemId, logId);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _workLogRepositoryMock.GetByIdTrackedAsync(logId, workItemId, Arg.Any<CancellationToken>())
            .Returns((WorkLog?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkLogNotFound);
        _workLogRepositoryMock.DidNotReceive().Remove(Arg.Any<WorkLog>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsNotOwner()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid logId = Guid.NewGuid();
        Guid ownerId = Guid.NewGuid();
        Guid currentUserId = Guid.NewGuid();
        DeleteWorkLogCommand command = new(projectId, workItemId, logId);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _userContextMock.UserId.Returns(currentUserId);

        WorkLog workLog = CreateWorkLog(ownerId);
        _workLogRepositoryMock.GetByIdTrackedAsync(logId, workItemId, Arg.Any<CancellationToken>())
            .Returns(workLog);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkLogNotOwner);
        _workLogRepositoryMock.DidNotReceive().Remove(Arg.Any<WorkLog>());
    }
}
