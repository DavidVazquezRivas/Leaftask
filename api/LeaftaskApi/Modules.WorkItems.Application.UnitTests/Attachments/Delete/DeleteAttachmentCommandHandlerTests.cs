using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.Attachments.Delete;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Attachments.Delete;

public class DeleteAttachmentCommandHandlerTests
{
    private readonly DeleteAttachmentCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepositoryMock;
    private readonly IAttachmentRepository _attachmentRepositoryMock;
    private readonly IFileStorage _fileStorageMock;
    private readonly IUserContext _userContextMock;

    public DeleteAttachmentCommandHandlerTests()
    {
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _attachmentRepositoryMock = Substitute.For<IAttachmentRepository>();
        _fileStorageMock = Substitute.For<IFileStorage>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new DeleteAttachmentCommandHandler(
            _workItemRepositoryMock,
            _attachmentRepositoryMock,
            _fileStorageMock,
            _userContextMock);
    }

    private static Attachment CreateAttachment(Guid userId)
    {
        ProjectReadModel project = new(Guid.NewGuid(), "TST");
        WorkItemStatus status = new(Guid.NewGuid(), "To Do");
        WorkItemType type = new(Guid.NewGuid(), "Task");
        WorkItem workItem = WorkItem.Create(1, "Title", "Desc", 3m, DateTime.UtcNow.AddDays(7), project, status, type);
        UserReadModel user = new(userId, "Alice", "Smith");
        return new Attachment(Guid.NewGuid(), "doc.pdf", new Uri("http://localhost:9000/b/obj"), DateTime.UtcNow, workItem, user);
    }

    [Fact]
    public async Task Handle_Should_DeleteAttachment_And_ReturnSuccess()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid attachmentId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        DeleteAttachmentCommand command = new(projectId, workItemId, attachmentId);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _userContextMock.UserId.Returns(userId);

        Attachment attachment = CreateAttachment(userId);
        _attachmentRepositoryMock.GetByIdTrackedAsync(attachmentId, workItemId, Arg.Any<CancellationToken>())
            .Returns(attachment);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _fileStorageMock.Received(1).DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        _attachmentRepositoryMock.Received(1).Remove(attachment);
        await _attachmentRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid attachmentId = Guid.NewGuid();
        DeleteAttachmentCommand command = new(projectId, workItemId, attachmentId);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);
        await _attachmentRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AttachmentNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid attachmentId = Guid.NewGuid();
        DeleteAttachmentCommand command = new(projectId, workItemId, attachmentId);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _attachmentRepositoryMock.GetByIdTrackedAsync(attachmentId, workItemId, Arg.Any<CancellationToken>())
            .Returns((Attachment?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.AttachmentNotFound);
        await _fileStorageMock.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsNotOwner()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid attachmentId = Guid.NewGuid();
        Guid ownerId = Guid.NewGuid();
        Guid currentUserId = Guid.NewGuid();
        DeleteAttachmentCommand command = new(projectId, workItemId, attachmentId);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _userContextMock.UserId.Returns(currentUserId);

        Attachment attachment = CreateAttachment(ownerId);
        _attachmentRepositoryMock.GetByIdTrackedAsync(attachmentId, workItemId, Arg.Any<CancellationToken>())
            .Returns(attachment);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.AttachmentNotOwner);
        await _fileStorageMock.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }
}
