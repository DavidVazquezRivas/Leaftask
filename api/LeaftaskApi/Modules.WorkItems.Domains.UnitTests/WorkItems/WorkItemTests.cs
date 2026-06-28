using BuildingBlocks.Domain.Events;
using FluentAssertions;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Events;

namespace Modules.WorkItems.Domains.UnitTests.WorkItems;

public class WorkItemTests
{
    private static WorkItem CreateWorkItem(
        Guid? projectId = null,
        string title = "Title",
        string description = "Desc",
        decimal estimation = 3m,
        WorkItemStatus? status = null,
        WorkItemType? type = null,
        UserReadModel? assignee = null,
        Guid? parentId = null)
    {
        ProjectReadModel project = new(projectId ?? Guid.NewGuid(), "TST");
        WorkItemStatus s = status ?? new WorkItemStatus(Guid.NewGuid(), "To Do");
        WorkItemType t = type ?? new WorkItemType(Guid.NewGuid(), "Task");
        return WorkItem.Create(1, title, description, estimation, DateTime.UtcNow.AddDays(7), project, s, t, assignee, parentId);
    }

    // -----------------------------------------------------------------------
    // Create
    // -----------------------------------------------------------------------

    [Fact]
    public void Create_Should_RaiseWorkItemCreatedDomainEvent()
    {
        // Arrange / Act
        WorkItem workItem = CreateWorkItem();

        // Assert
        workItem.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkItemCreatedDomainEvent>();
    }

    [Fact]
    public void Create_Should_SetProperties()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        WorkItemStatus status = new(Guid.NewGuid(), "To Do");
        WorkItemType type = new(Guid.NewGuid(), "Task");

        // Act
        WorkItem workItem = CreateWorkItem(projectId: projectId, title: "My Title",
            description: "My Desc", estimation: 5m, status: status, type: type);

        // Assert
        workItem.Title.Should().Be("My Title");
        workItem.Description.Should().Be("My Desc");
        workItem.Estimation.Should().Be(5m);
        workItem.Progress.Should().Be(0);
        workItem.Project.Id.Should().Be(projectId);
        workItem.Status.Id.Should().Be(status.Id);
        workItem.Type.Id.Should().Be(type.Id);
    }

    // -----------------------------------------------------------------------
    // Delete
    // -----------------------------------------------------------------------

    [Fact]
    public void Delete_Should_RaiseWorkItemDeletedDomainEvent()
    {
        // Arrange
        WorkItem workItem = CreateWorkItem();
        workItem.ClearDomainEvents();

        // Act
        workItem.Delete();

        // Assert
        workItem.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<WorkItemDeletedDomainEvent>();
    }

    [Fact]
    public void Delete_Should_RaiseEventWithCorrectIds()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        WorkItem workItem = CreateWorkItem(projectId: projectId);
        workItem.ClearDomainEvents();

        // Act
        workItem.Delete();

        // Assert
        WorkItemDeletedDomainEvent evt = workItem.DomainEvents
            .OfType<WorkItemDeletedDomainEvent>()
            .Single();

        evt.WorkItemId.Should().Be(workItem.Id);
        evt.ProjectId.Should().Be(projectId);
    }

    // -----------------------------------------------------------------------
    // AddComment
    // -----------------------------------------------------------------------

    [Fact]
    public void AddComment_Should_ReturnWorkItemComment_WithCorrectContent()
    {
        // Arrange
        WorkItem workItem = CreateWorkItem();
        workItem.ClearDomainEvents();
        UserReadModel author = new(Guid.NewGuid(), "Alice", "Smith");

        // Act
        WorkItemComment comment = workItem.AddComment("Hello world", author, []);

        // Assert
        comment.Should().NotBeNull();
        comment.Content.Should().Be("Hello world");
        comment.User.Id.Should().Be(author.Id);
        comment.WorkItem.Should().BeSameAs(workItem);
    }

    [Fact]
    public void AddComment_Should_RaiseCommentAddedDomainEvent()
    {
        // Arrange
        WorkItem workItem = CreateWorkItem();
        workItem.ClearDomainEvents();
        UserReadModel author = new(Guid.NewGuid(), "Alice", "Smith");
        Guid mentionedUserId = Guid.NewGuid();

        // Act
        WorkItemComment comment = workItem.AddComment("Hi", author, [mentionedUserId]);

        // Assert
        workItem.DomainEvents.Should().Contain(e => e is CommentAddedDomainEvent);
        CommentAddedDomainEvent evt = workItem.DomainEvents
            .OfType<CommentAddedDomainEvent>()
            .Single();

        evt.WorkItemId.Should().Be(workItem.Id);
        evt.CommentId.Should().Be(comment.Id);
        evt.AuthorId.Should().Be(author.Id);
        evt.MentionedUserIds.Should().Contain(mentionedUserId);
    }

    [Fact]
    public void AddComment_Should_RaiseWorkItemCommentAddedDomainEvent()
    {
        // Arrange
        WorkItem workItem = CreateWorkItem();
        workItem.ClearDomainEvents();
        UserReadModel author = new(Guid.NewGuid(), "Bob", "Jones");

        // Act
        WorkItemComment comment = workItem.AddComment("Text", author, []);

        // Assert
        workItem.DomainEvents.Should().Contain(e => e is WorkItemCommentAddedDomainEvent);
        WorkItemCommentAddedDomainEvent evt = workItem.DomainEvents
            .OfType<WorkItemCommentAddedDomainEvent>()
            .Single();

        evt.WorkItemId.Should().Be(workItem.Id);
        evt.CommentId.Should().Be(comment.Id);
        evt.AuthorId.Should().Be(author.Id);
        evt.ProjectId.Should().Be(workItem.Project.Id);
    }

    // -----------------------------------------------------------------------
    // ApplyUpdate — title / description / estimation / limitDate
    // -----------------------------------------------------------------------

    [Fact]
    public void ApplyUpdate_Should_ReturnEmptyChanges_When_NothingChanges()
    {
        // Arrange
        WorkItem workItem = CreateWorkItem(title: "Same", description: "Same");
        workItem.ClearDomainEvents();

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            "Same", "Same", null, null, null, null, null, null, false, null, false);

        // Assert
        changes.Should().BeEmpty();
    }

    [Fact]
    public void ApplyUpdate_Should_UpdateTitle_And_ReturnChange()
    {
        // Arrange
        WorkItem workItem = CreateWorkItem(title: "Old Title");

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            "New Title", null, null, null, null, null, null, null, false, null, false);

        // Assert
        changes.Should().ContainSingle(c => c.FieldName == "title");
        WorkItemChange change = changes.First(c => c.FieldName == "title");
        change.OldValue.Should().Be("Old Title");
        change.NewValue.Should().Be("New Title");
        workItem.Title.Should().Be("New Title");
    }

    [Fact]
    public void ApplyUpdate_Should_UpdateDescription_And_ReturnChange()
    {
        // Arrange
        WorkItem workItem = CreateWorkItem(description: "Old Desc");

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            null, "New Desc", null, null, null, null, null, null, false, null, false);

        // Assert
        changes.Should().ContainSingle(c => c.FieldName == "description");
        workItem.Description.Should().Be("New Desc");
    }

    [Fact]
    public void ApplyUpdate_Should_UpdateProgress_And_RaiseProgressEvent()
    {
        // Arrange
        WorkItem workItem = CreateWorkItem();
        workItem.ClearDomainEvents();

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            null, null, 50, null, null, null, null, null, false, null, false);

        // Assert
        changes.Should().ContainSingle(c => c.FieldName == "progress");
        workItem.Progress.Should().Be(50);
        workItem.DomainEvents.Should().Contain(e => e is WorkItemProgressUpdatedDomainEvent);
    }

    [Fact]
    public void ApplyUpdate_Should_UpdateEstimation_And_ReturnChange()
    {
        // Arrange
        WorkItem workItem = CreateWorkItem(estimation: 3m);

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            null, null, null, 8m, null, null, null, null, false, null, false);

        // Assert
        changes.Should().ContainSingle(c => c.FieldName == "estimation");
        workItem.Estimation.Should().Be(8m);
    }

    [Fact]
    public void ApplyUpdate_Should_UpdateLimitDate_And_ReturnChange()
    {
        // Arrange
        WorkItem workItem = CreateWorkItem();
        DateTime newDate = DateTime.UtcNow.AddDays(30);

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            null, null, null, null, newDate, null, null, null, false, null, false);

        // Assert
        changes.Should().ContainSingle(c => c.FieldName == "limitDate");
        workItem.LimitDate.Date.Should().Be(newDate.Date);
    }

    [Fact]
    public void ApplyUpdate_Should_UpdateStatus_And_RaiseStatusChangedEvent()
    {
        // Arrange
        WorkItemStatus originalStatus = new(Guid.NewGuid(), "To Do");
        WorkItemStatus newStatus = new(Guid.NewGuid(), "In Progress");
        WorkItem workItem = CreateWorkItem(status: originalStatus);
        workItem.ClearDomainEvents();

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            null, null, null, null, null, newStatus, null, null, false, null, false);

        // Assert
        changes.Should().ContainSingle(c => c.FieldName == "statusId");
        workItem.Status.Id.Should().Be(newStatus.Id);
        workItem.DomainEvents.Should().Contain(e => e is WorkItemStatusChangedDomainEvent);
        WorkItemStatusChangedDomainEvent evt = workItem.DomainEvents
            .OfType<WorkItemStatusChangedDomainEvent>()
            .Single();
        evt.OldStatusId.Should().Be(originalStatus.Id);
        evt.NewStatusId.Should().Be(newStatus.Id);
    }

    [Fact]
    public void ApplyUpdate_Should_UpdateType_And_ReturnChange()
    {
        // Arrange
        WorkItemType originalType = new(Guid.NewGuid(), "Task");
        WorkItemType newType = new(Guid.NewGuid(), "Bug");
        WorkItem workItem = CreateWorkItem(type: originalType);

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            null, null, null, null, null, null, newType, null, false, null, false);

        // Assert
        changes.Should().ContainSingle(c => c.FieldName == "typeId");
        workItem.Type.Id.Should().Be(newType.Id);
    }

    [Fact]
    public void ApplyUpdate_Should_UpdateAssignee_When_UpdateAssigneeIsTrue()
    {
        // Arrange
        UserReadModel newAssignee = new(Guid.NewGuid(), "Jane", "Doe");
        WorkItem workItem = CreateWorkItem();
        workItem.ClearDomainEvents();

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            null, null, null, null, null, null, null, newAssignee, true, null, false);

        // Assert
        changes.Should().ContainSingle(c => c.FieldName == "assigneeId");
        workItem.Asignee.Should().BeSameAs(newAssignee);
        workItem.DomainEvents.Should().Contain(e => e is WorkItemAssigneeChangedDomainEvent);
    }

    [Fact]
    public void ApplyUpdate_Should_NotUpdateAssignee_When_UpdateAssigneeIsFalse()
    {
        // Arrange
        UserReadModel existingAssignee = new(Guid.NewGuid(), "Old", "Person");
        WorkItem workItem = CreateWorkItem(assignee: existingAssignee);
        UserReadModel newAssignee = new(Guid.NewGuid(), "New", "Person");

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            null, null, null, null, null, null, null, newAssignee, false, null, false);

        // Assert
        changes.Should().NotContain(c => c.FieldName == "assigneeId");
        workItem.Asignee.Should().BeSameAs(existingAssignee);
    }

    [Fact]
    public void ApplyUpdate_Should_UpdateParentId_When_UpdateParentIsTrue()
    {
        // Arrange
        WorkItem workItem = CreateWorkItem();
        Guid newParentId = Guid.NewGuid();

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            null, null, null, null, null, null, null, null, false, newParentId, true);

        // Assert
        changes.Should().ContainSingle(c => c.FieldName == "parentId");
        workItem.ParentId.Should().Be(newParentId);
    }

    [Fact]
    public void ApplyUpdate_Should_NotUpdateParentId_When_UpdateParentIsFalse()
    {
        // Arrange
        Guid originalParentId = Guid.NewGuid();
        WorkItem workItem = CreateWorkItem(parentId: originalParentId);
        Guid newParentId = Guid.NewGuid();

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            null, null, null, null, null, null, null, null, false, newParentId, false);

        // Assert
        changes.Should().NotContain(c => c.FieldName == "parentId");
        workItem.ParentId.Should().Be(originalParentId);
    }

    [Fact]
    public void ApplyUpdate_Should_ReturnMultipleChanges_When_MultipleFieldsChange()
    {
        // Arrange
        WorkItem workItem = CreateWorkItem(title: "Old", description: "Old desc");

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            "New Title", "New Desc", null, null, null, null, null, null, false, null, false);

        // Assert
        changes.Should().HaveCount(2);
        changes.Should().Contain(c => c.FieldName == "title");
        changes.Should().Contain(c => c.FieldName == "description");
    }

    [Fact]
    public void ApplyUpdate_Should_NotRaiseStatusEvent_When_StatusDoesNotChange()
    {
        // Arrange
        WorkItemStatus status = new(Guid.NewGuid(), "To Do");
        WorkItem workItem = CreateWorkItem(status: status);
        workItem.ClearDomainEvents();

        // Act
        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            null, null, null, null, null, status, null, null, false, null, false);

        // Assert
        changes.Should().NotContain(c => c.FieldName == "statusId");
        workItem.DomainEvents.Should().NotContain(e => e is WorkItemStatusChangedDomainEvent);
    }
}
