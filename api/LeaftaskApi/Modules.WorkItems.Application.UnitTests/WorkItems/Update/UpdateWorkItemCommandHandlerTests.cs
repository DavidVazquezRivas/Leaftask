using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.WorkItems;
using Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;
using Modules.WorkItems.Application.WorkItems.Update;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.WorkItems.Update;

public class UpdateWorkItemCommandHandlerTests
{
    private readonly UpdateWorkItemCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepositoryMock;
    private readonly IWorkItemConfigurationRepository _configurationRepositoryMock;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;
    private readonly IFieldRepository _fieldRepositoryMock;
    private readonly IGetWorkItemDetailsQueryService _detailsServiceMock;
    private readonly IUserContext _userContextMock;

    public UpdateWorkItemCommandHandlerTests()
    {
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _configurationRepositoryMock = Substitute.For<IWorkItemConfigurationRepository>();
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();
        _fieldRepositoryMock = Substitute.For<IFieldRepository>();
        _detailsServiceMock = Substitute.For<IGetWorkItemDetailsQueryService>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new UpdateWorkItemCommandHandler(
            _workItemRepositoryMock,
            _configurationRepositoryMock,
            _userReadModelRepositoryMock,
            _fieldRepositoryMock,
            _detailsServiceMock,
            _userContextMock);
    }

    private static WorkItem CreateWorkItem(Guid projectId)
    {
        ProjectReadModel project = new(projectId, "TST");
        WorkItemStatus status = new(Guid.NewGuid(), "To Do");
        WorkItemType type = new(Guid.NewGuid(), "Task");
        return WorkItem.Create(1, "Title", "Desc", 3m, DateTime.UtcNow.AddDays(7), project, status, type);
    }

    private static WorkItemDetailDto BuildDto() =>
        new(Guid.NewGuid(), "TST-1", "Updated", null, null, null, null,
            new WorkItemDedicationDto(0, 0), 0,
            Guid.NewGuid(), "Task", Guid.NewGuid(), "To Do", null,
            [], [], [], []);

    [Fact]
    public async Task Handle_Should_ReturnUpdatedDetails_When_WorkItemExists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        WorkItem workItem = CreateWorkItem(projectId);
        WorkItemDetailDto detail = BuildDto();

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _userContextMock.UserId.Returns(userId);
        _detailsServiceMock.GetWorkItemDetailsAsync(projectId, workItemId, Arg.Any<CancellationToken>())
            .Returns(detail);

        // (ProjectId, WorkItemId, Title, Desc, StatusId, TypeId, AssigneeId, UpdateAssignee, Progress, Estimation, LimitDate, ParentId, UpdateParent, CustomFields)
        UpdateWorkItemCommand command = new(projectId, workItemId, "Updated", null, null, null, null, false, null, null, null, null, false, new Dictionary<Guid, string>());

        // Act
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(detail);
        await _workItemRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns((WorkItem?)null);

        UpdateWorkItemCommand command = new(projectId, workItemId, null, null, null, null, null, false, null, null, null, null, false, new Dictionary<Guid, string>());

        // Act
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_StatusNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid statusId = Guid.NewGuid();
        WorkItem workItem = CreateWorkItem(projectId);

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _configurationRepositoryMock.GetStatusByIdAsync(statusId, Arg.Any<CancellationToken>())
            .Returns((WorkItemStatus?)null);

        UpdateWorkItemCommand command = new(projectId, workItemId, null, null, statusId, null, null, false, null, null, null, null, false, new Dictionary<Guid, string>());

        // Act
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.StatusNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TypeNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        WorkItem workItem = CreateWorkItem(projectId);

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _configurationRepositoryMock.GetTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns((WorkItemType?)null);

        UpdateWorkItemCommand command = new(projectId, workItemId, null, null, null, typeId, null, false, null, null, null, null, false, new Dictionary<Guid, string>());

        // Act
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.TypeNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AssigneeNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid assigneeId = Guid.NewGuid();
        WorkItem workItem = CreateWorkItem(projectId);

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _userReadModelRepositoryMock.GetByIdAsync(assigneeId, Arg.Any<CancellationToken>())
            .Returns((UserReadModel?)null);

        UpdateWorkItemCommand command = new(projectId, workItemId, null, null, null, null, assigneeId, true, null, null, null, null, false, new Dictionary<Guid, string>());

        // Act
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.AssigneeNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ParentNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid parentId = Guid.NewGuid();
        WorkItem workItem = CreateWorkItem(projectId);

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _workItemRepositoryMock.ExistsInProjectAsync(parentId, projectId, Arg.Any<CancellationToken>())
            .Returns(false);

        UpdateWorkItemCommand command = new(projectId, workItemId, null, null, null, null, null, false, null, null, null, parentId, true, new Dictionary<Guid, string>());

        // Act
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.ParentNotFound);
    }

    [Fact]
    public async Task Handle_Should_AddActivityLog_When_ChangesOccurAndActorFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        WorkItem workItem = CreateWorkItem(projectId);
        WorkItemDetailDto detail = BuildDto();
        UserReadModel actor = new(userId, "Actor", "User");

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _userContextMock.UserId.Returns(userId);
        _userReadModelRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(actor);
        _detailsServiceMock.GetWorkItemDetailsAsync(projectId, workItemId, Arg.Any<CancellationToken>())
            .Returns(detail);

        // Title changes from "Title" to "Changed Title" → 1 change
        UpdateWorkItemCommand command = new(projectId, workItemId, "Changed Title", null, null, null, null, false, null, null, null, null, false, new Dictionary<Guid, string>());

        // Act
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _workItemRepositoryMock.Received(1)
            .AddActivityLogAsync(Arg.Any<ActivityLog>(), Arg.Any<CancellationToken>());
    }
}
