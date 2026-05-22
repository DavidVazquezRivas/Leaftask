using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.WorkLogs;
using Modules.WorkItems.Application.WorkLogs.Create;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.WorkLogs.Create;

public class LogWorkCommandHandlerTests
{
    private readonly LogWorkCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepositoryMock;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;
    private readonly IWorkLogRepository _workLogRepositoryMock;
    private readonly IUserContext _userContextMock;

    public LogWorkCommandHandlerTests()
    {
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();
        _workLogRepositoryMock = Substitute.For<IWorkLogRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new LogWorkCommandHandler(
            _workItemRepositoryMock,
            _userReadModelRepositoryMock,
            _workLogRepositoryMock,
            _userContextMock);
    }

    private static WorkItem CreateWorkItem(Guid projectId)
    {
        ProjectReadModel project = new(projectId, "TST");
        WorkItemStatus status = new(Guid.NewGuid(), "To Do");
        WorkItemType type = new(Guid.NewGuid(), "Task");
        return WorkItem.Create(1, "Title", "Desc", 3m, DateTime.UtcNow.AddDays(7), project, status, type);
    }

    [Fact]
    public async Task Handle_Should_CreateWorkLog_And_ReturnSuccess()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        LogWorkCommand command = new(projectId, workItemId, 2.5m, new DateOnly(2026, 5, 1), "Worked on feature");

        WorkItem workItem = CreateWorkItem(projectId);
        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(workItem);

        _userContextMock.UserId.Returns(userId);
        UserReadModel user = new(userId, "Alice", "Smith");
        _userReadModelRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        Result<WorkLogDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Dedication.Should().BeApproximately(2.5f, 0.001f);
        result.Value.Date.Should().Be(new DateOnly(2026, 5, 1));
        result.Value.Description.Should().Be("Worked on feature");
        result.Value.User.FirstName.Should().Be("Alice");

        await _workLogRepositoryMock.Received(1).AddAsync(Arg.Any<WorkLog>(), Arg.Any<CancellationToken>());
        await _workLogRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        LogWorkCommand command = new(projectId, workItemId, 1m, DateOnly.FromDateTime(DateTime.Today), "Work");

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns((WorkItem?)null);

        // Act
        Result<WorkLogDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);
        await _workLogRepositoryMock.DidNotReceive().AddAsync(Arg.Any<WorkLog>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        LogWorkCommand command = new(projectId, workItemId, 1m, DateOnly.FromDateTime(DateTime.Today), "Work");

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(CreateWorkItem(projectId));

        _userContextMock.UserId.Returns(userId);
        _userReadModelRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((UserReadModel?)null);

        // Act
        Result<WorkLogDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.AssigneeNotFound);
        await _workLogRepositoryMock.DidNotReceive().AddAsync(Arg.Any<WorkLog>(), Arg.Any<CancellationToken>());
    }
}
