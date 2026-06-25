using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.Comments;
using Modules.WorkItems.Application.Comments.Update;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Comments.Update;

public class UpdateCommentCommandHandlerTests
{
    private readonly UpdateCommentCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepositoryMock;
    private readonly IAttachmentRepository _attachmentRepositoryMock;
    private readonly ICommentRepository _commentRepositoryMock;
    private readonly IUserContext _userContextMock;

    public UpdateCommentCommandHandlerTests()
    {
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _attachmentRepositoryMock = Substitute.For<IAttachmentRepository>();
        _commentRepositoryMock = Substitute.For<ICommentRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new UpdateCommentCommandHandler(
            _workItemRepositoryMock,
            _attachmentRepositoryMock,
            _commentRepositoryMock,
            _userContextMock);
    }

    private static WorkItemComment CreateComment(Guid userId)
    {
        ProjectReadModel project = new(Guid.NewGuid(), "TST");
        WorkItemStatus status = new(Guid.NewGuid(), "To Do");
        WorkItemType type = new(Guid.NewGuid(), "Task");
        WorkItem workItem = WorkItem.Create(1, "Title", "Desc", 0m, DateTime.UtcNow.AddDays(7), project, status, type);
        UserReadModel user = new(userId, "John", "Doe");
        return new WorkItemComment(Guid.NewGuid(), "Original", workItem, user);
    }

    [Fact]
    public async Task Handle_Should_UpdateContent_And_ReturnCommentDto()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid commentId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        UpdateCommentCommand command = new(projectId, workItemId, commentId, "Updated content", null);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _userContextMock.UserId.Returns(userId);

        WorkItemComment comment = CreateComment(userId);
        _commentRepositoryMock.GetByIdTrackedAsync(commentId, workItemId, Arg.Any<CancellationToken>())
            .Returns(comment);

        _attachmentRepositoryMock.GetByCommentIdTrackedAsync(commentId, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result<CommentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Content.Should().Be("Updated content");
        await _commentRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid commentId = Guid.NewGuid();
        UpdateCommentCommand command = new(projectId, workItemId, commentId, "Content", null);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result<CommentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);
        await _commentRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CommentNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid commentId = Guid.NewGuid();
        UpdateCommentCommand command = new(projectId, workItemId, commentId, "Content", null);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _commentRepositoryMock.GetByIdTrackedAsync(commentId, workItemId, Arg.Any<CancellationToken>())
            .Returns((WorkItemComment?)null);

        // Act
        Result<CommentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.CommentNotFound);
        await _commentRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsNotOwner()
    {
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid commentId = Guid.NewGuid();
        Guid ownerId = Guid.NewGuid();
        Guid currentUserId = Guid.NewGuid();

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>()).Returns(true);
        _userContextMock.UserId.Returns(currentUserId);
        _commentRepositoryMock.GetByIdTrackedAsync(commentId, workItemId, Arg.Any<CancellationToken>())
            .Returns(CreateComment(ownerId));

        Result<CommentDto> result = await _handler.Handle(
            new(projectId, workItemId, commentId, "Content", null), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.CommentNotOwner);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AttachmentIdsProvided_But_SomeNotFound()
    {
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid commentId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        List<Guid> attachmentIds = [Guid.NewGuid(), Guid.NewGuid()];

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>()).Returns(true);
        _userContextMock.UserId.Returns(userId);
        _commentRepositoryMock.GetByIdTrackedAsync(commentId, workItemId, Arg.Any<CancellationToken>())
            .Returns(CreateComment(userId));
        _attachmentRepositoryMock.GetByCommentIdTrackedAsync(commentId, Arg.Any<CancellationToken>())
            .Returns([]);
        _attachmentRepositoryMock.GetByIdsTrackedAsync(attachmentIds, workItemId, Arg.Any<CancellationToken>())
            .Returns([]);  // Only 0 found, but 2 requested → mismatch

        Result<CommentDto> result = await _handler.Handle(
            new(projectId, workItemId, commentId, null, attachmentIds), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.AttachmentNotFound);
    }

    [Fact]
    public async Task Handle_Should_ClearAttachments_When_EmptyAttachmentIds()
    {
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid commentId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>()).Returns(true);
        _userContextMock.UserId.Returns(userId);
        _commentRepositoryMock.GetByIdTrackedAsync(commentId, workItemId, Arg.Any<CancellationToken>())
            .Returns(CreateComment(userId));
        _attachmentRepositoryMock.GetByCommentIdTrackedAsync(commentId, Arg.Any<CancellationToken>())
            .Returns([]);

        Result<CommentDto> result = await _handler.Handle(
            new(projectId, workItemId, commentId, null, []), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Attachments.Should().BeEmpty();
    }
}
