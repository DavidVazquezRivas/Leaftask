using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.WorkLogs;
using Modules.WorkItems.Application.WorkLogs.Update;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.WorkLogs.Update;

public class UpdateWorkLogCommandHandlerTests
{
    private readonly UpdateWorkLogCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepositoryMock;
    private readonly IWorkLogRepository _workLogRepositoryMock;
    private readonly IUserContext _userContextMock;

    public UpdateWorkLogCommandHandlerTests()
    {
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _workLogRepositoryMock = Substitute.For<IWorkLogRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new UpdateWorkLogCommandHandler(
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

        return new WorkLog(Guid.NewGuid(), DateTime.UtcNow, 2m, "Original", workItem, user);
    }

    [Fact]
    public async Task Handle_Should_UpdateWorkLog_And_ReturnSuccess()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid logId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        UpdateWorkLogCommand command = new(projectId, workItemId, logId, 4m, new DateOnly(2026, 6, 1), "Updated work");

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _userContextMock.UserId.Returns(userId);

        WorkLog workLog = CreateWorkLog(userId);
        _workLogRepositoryMock.GetByIdTrackedAsync(logId, workItemId, Arg.Any<CancellationToken>())
            .Returns(workLog);

        // Act
        Result<WorkLogDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Dedication.Should().BeApproximately(4f, 0.001f);
        result.Value.Date.Should().Be(new DateOnly(2026, 6, 1));
        result.Value.Description.Should().Be("Updated work");

        await _workLogRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid logId = Guid.NewGuid();
        UpdateWorkLogCommand command = new(projectId, workItemId, logId, 1m, null, null);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result<WorkLogDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);
        await _workLogRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkLogNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid logId = Guid.NewGuid();
        UpdateWorkLogCommand command = new(projectId, workItemId, logId, 1m, null, null);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _workLogRepositoryMock.GetByIdTrackedAsync(logId, workItemId, Arg.Any<CancellationToken>())
            .Returns((WorkLog?)null);

        // Act
        Result<WorkLogDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkLogNotFound);
        await _workLogRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
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
        UpdateWorkLogCommand command = new(projectId, workItemId, logId, 1m, null, null);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _userContextMock.UserId.Returns(currentUserId);

        WorkLog workLog = CreateWorkLog(ownerId);
        _workLogRepositoryMock.GetByIdTrackedAsync(logId, workItemId, Arg.Any<CancellationToken>())
            .Returns(workLog);

        // Act
        Result<WorkLogDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkLogNotOwner);
        await _workLogRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
