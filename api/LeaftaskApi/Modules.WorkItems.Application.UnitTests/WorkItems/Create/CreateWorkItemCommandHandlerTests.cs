using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.WorkItems;
using Modules.WorkItems.Application.WorkItems.Create;
using Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Field;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.WorkItems.Create;

public class CreateWorkItemCommandHandlerTests
{
    private readonly CreateWorkItemCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepositoryMock;
    private readonly IProjectReadModelRepository _projectReadModelRepositoryMock;
    private readonly IWorkItemConfigurationRepository _configurationRepositoryMock;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;
    private readonly IFieldRepository _fieldRepositoryMock;
    private readonly IGetWorkItemDetailsQueryService _detailsServiceMock;

    public CreateWorkItemCommandHandlerTests()
    {
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _projectReadModelRepositoryMock = Substitute.For<IProjectReadModelRepository>();
        _configurationRepositoryMock = Substitute.For<IWorkItemConfigurationRepository>();
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();
        _fieldRepositoryMock = Substitute.For<IFieldRepository>();
        _detailsServiceMock = Substitute.For<IGetWorkItemDetailsQueryService>();

        _handler = new CreateWorkItemCommandHandler(
            _workItemRepositoryMock,
            _projectReadModelRepositoryMock,
            _configurationRepositoryMock,
            _userReadModelRepositoryMock,
            _fieldRepositoryMock,
            _detailsServiceMock);
    }

    private static CreateWorkItemCommand BuildCommand(Guid projectId, Guid statusId, Guid typeId) =>
        new(projectId, "Title", "Desc", 3m, typeId, statusId, null, projectId, new Dictionary<Guid, string>());

    private static WorkItemDetailDto BuildDto() =>
        new(Guid.NewGuid(), "TST-1", "Title", null, null, null, null,
            new WorkItemDedicationDto(0, 0), 0,
            Guid.NewGuid(), "Task", Guid.NewGuid(), "To Do", null,
            [], [], [], []);

    [Fact]
    public async Task Handle_Should_CreateWorkItem_And_ReturnDetails()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid statusId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        WorkItemDetailDto detail = BuildDto();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _configurationRepositoryMock.GetStatusByIdAsync(statusId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemStatus(statusId, "To Do"));
        _configurationRepositoryMock.GetTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemType(typeId, "Task"));
        _workItemRepositoryMock.GetNextCodeAsync(projectId, Arg.Any<CancellationToken>()).Returns(1);
        _detailsServiceMock.GetWorkItemDetailsAsync(projectId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(detail);

        CreateWorkItemCommand command = BuildCommand(projectId, statusId, typeId);

        // Act
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(detail);
        await _workItemRepositoryMock.Received(1).AddAsync(Arg.Any<WorkItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((ProjectReadModel?)null);
        CreateWorkItemCommand command = BuildCommand(projectId, Guid.NewGuid(), Guid.NewGuid());

        // Act
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.ProjectNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_StatusNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid statusId = Guid.NewGuid();
        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _configurationRepositoryMock.GetStatusByIdAsync(statusId, Arg.Any<CancellationToken>())
            .Returns((WorkItemStatus?)null);
        CreateWorkItemCommand command = BuildCommand(projectId, statusId, Guid.NewGuid());

        // Act
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.StatusNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TypeNotFound()
    {
        Guid projectId = Guid.NewGuid();
        Guid statusId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _configurationRepositoryMock.GetStatusByIdAsync(statusId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemStatus(statusId, "To Do"));
        _configurationRepositoryMock.GetTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns((WorkItemType?)null);

        Result<WorkItemDetailDto> result = await _handler.Handle(BuildCommand(projectId, statusId, typeId), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.TypeNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AssigneeNotFound()
    {
        Guid projectId = Guid.NewGuid();
        Guid statusId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        Guid assigneeId = Guid.NewGuid();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _configurationRepositoryMock.GetStatusByIdAsync(statusId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemStatus(statusId, "To Do"));
        _configurationRepositoryMock.GetTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemType(typeId, "Task"));
        _userReadModelRepositoryMock.GetByIdAsync(assigneeId, Arg.Any<CancellationToken>())
            .Returns((UserReadModel?)null);

        CreateWorkItemCommand command = new(projectId, "Title", "Desc", 3m, typeId, statusId, assigneeId, projectId, new Dictionary<Guid, string>());
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.AssigneeNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ParentNotFound()
    {
        Guid projectId = Guid.NewGuid();
        Guid statusId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        Guid parentId = Guid.NewGuid();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _configurationRepositoryMock.GetStatusByIdAsync(statusId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemStatus(statusId, "To Do"));
        _configurationRepositoryMock.GetTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemType(typeId, "Task"));
        _workItemRepositoryMock.ExistsInProjectAsync(parentId, projectId, Arg.Any<CancellationToken>()).Returns(false);

        CreateWorkItemCommand command = new(projectId, "Title", "Desc", 3m, typeId, statusId, null, parentId, new Dictionary<Guid, string>());
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.ParentNotFound);
    }

    [Fact]
    public async Task Handle_Should_CreateWorkItem_WithParentId_When_ParentExists()
    {
        Guid projectId = Guid.NewGuid();
        Guid statusId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        Guid parentId = Guid.NewGuid();
        WorkItemDetailDto detail = BuildDto();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _configurationRepositoryMock.GetStatusByIdAsync(statusId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemStatus(statusId, "To Do"));
        _configurationRepositoryMock.GetTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemType(typeId, "Task"));
        _workItemRepositoryMock.ExistsInProjectAsync(parentId, projectId, Arg.Any<CancellationToken>()).Returns(true);
        _workItemRepositoryMock.GetNextCodeAsync(projectId, Arg.Any<CancellationToken>()).Returns(2);
        _detailsServiceMock.GetWorkItemDetailsAsync(projectId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(detail);

        CreateWorkItemCommand command = new(projectId, "Title", "Desc", 3m, typeId, statusId, null, parentId, new Dictionary<Guid, string>());
        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ApplyCustomFields_When_CustomFieldsProvided()
    {
        Guid projectId = Guid.NewGuid();
        Guid statusId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        Guid fieldId = Guid.NewGuid();
        WorkItemDetailDto detail = BuildDto();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _configurationRepositoryMock.GetStatusByIdAsync(statusId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemStatus(statusId, "To Do"));
        _configurationRepositoryMock.GetTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemType(typeId, "Task"));
        _workItemRepositoryMock.GetNextCodeAsync(projectId, Arg.Any<CancellationToken>()).Returns(1);
        _fieldRepositoryMock.GetFieldValuesForWorkItemAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<FieldValue>());
        _fieldRepositoryMock.GetFieldReadModelTrackedByIdAsync(fieldId, Arg.Any<CancellationToken>())
            .Returns(new FieldReadModel(fieldId, "Notes", isOptional: true, new FieldTypeReadModel(Guid.NewGuid(), "Texto")));
        _detailsServiceMock.GetWorkItemDetailsAsync(projectId, Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(detail);

        CreateWorkItemCommand command = new(projectId, "Title", "Desc", 3m, typeId, statusId, null, projectId,
            new Dictionary<Guid, string> { [fieldId] = "some value" });

        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _fieldRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_RequiredCustomFieldHasEmptyValue()
    {
        Guid projectId = Guid.NewGuid();
        Guid statusId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        Guid fieldId = Guid.NewGuid();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _configurationRepositoryMock.GetStatusByIdAsync(statusId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemStatus(statusId, "To Do"));
        _configurationRepositoryMock.GetTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemType(typeId, "Task"));
        _workItemRepositoryMock.GetNextCodeAsync(projectId, Arg.Any<CancellationToken>()).Returns(1);
        _fieldRepositoryMock.GetFieldValuesForWorkItemAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<FieldValue>());
        _fieldRepositoryMock.GetFieldReadModelTrackedByIdAsync(fieldId, Arg.Any<CancellationToken>())
            .Returns(new FieldReadModel(fieldId, "Priority", isOptional: false, new FieldTypeReadModel(Guid.NewGuid(), "Texto")));

        CreateWorkItemCommand command = new(projectId, "Title", "Desc", 3m, typeId, statusId, null, projectId,
            new Dictionary<Guid, string> { [fieldId] = "   " });

        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.RequiredFieldValueMissing);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NumberFieldHasInvalidValue()
    {
        Guid projectId = Guid.NewGuid();
        Guid statusId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        Guid fieldId = Guid.NewGuid();

        _projectReadModelRepositoryMock.GetByIdAsync(projectId, Arg.Any<CancellationToken>())
            .Returns(new ProjectReadModel(projectId, "TST"));
        _configurationRepositoryMock.GetStatusByIdAsync(statusId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemStatus(statusId, "To Do"));
        _configurationRepositoryMock.GetTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns(new WorkItemType(typeId, "Task"));
        _workItemRepositoryMock.GetNextCodeAsync(projectId, Arg.Any<CancellationToken>()).Returns(1);
        _fieldRepositoryMock.GetFieldValuesForWorkItemAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<FieldValue>());
        _fieldRepositoryMock.GetFieldReadModelTrackedByIdAsync(fieldId, Arg.Any<CancellationToken>())
            .Returns(new FieldReadModel(fieldId, "Score", isOptional: true, new FieldTypeReadModel(Guid.NewGuid(), "Número")));

        CreateWorkItemCommand command = new(projectId, "Title", "Desc", 3m, typeId, statusId, null, projectId,
            new Dictionary<Guid, string> { [fieldId] = "not-a-number" });

        Result<WorkItemDetailDto> result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.InvalidFieldValue);
    }
}
