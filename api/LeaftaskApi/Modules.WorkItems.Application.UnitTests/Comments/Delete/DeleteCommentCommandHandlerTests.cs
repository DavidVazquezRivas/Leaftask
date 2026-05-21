using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.Comments.Delete;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Comments.Delete;

public class DeleteCommentCommandHandlerTests
{
    private readonly DeleteCommentCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepositoryMock;
    private readonly IAttachmentRepository _attachmentRepositoryMock;
    private readonly ICommentRepository _commentRepositoryMock;
    private readonly IUserContext _userContextMock;

    public DeleteCommentCommandHandlerTests()
    {
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _attachmentRepositoryMock = Substitute.For<IAttachmentRepository>();
        _commentRepositoryMock = Substitute.For<ICommentRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new DeleteCommentCommandHandler(
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
        return new WorkItemComment(Guid.NewGuid(), "Content", workItem, user);
    }

    [Fact]
    public async Task Handle_Should_DeleteComment_When_UserIsOwner()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid commentId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        DeleteCommentCommand command = new(projectId, workItemId, commentId);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _userContextMock.UserId.Returns(userId);

        WorkItemComment comment = CreateComment(userId);
        _commentRepositoryMock.GetByIdTrackedAsync(commentId, workItemId, Arg.Any<CancellationToken>())
            .Returns(comment);

        _attachmentRepositoryMock.GetByCommentIdTrackedAsync(commentId, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _commentRepositoryMock.Received(1).Remove(comment);
        await _commentRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid commentId = Guid.NewGuid();
        DeleteCommentCommand command = new(projectId, workItemId, commentId);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

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
        DeleteCommentCommand command = new(projectId, workItemId, commentId);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _commentRepositoryMock.GetByIdTrackedAsync(commentId, workItemId, Arg.Any<CancellationToken>())
            .Returns((WorkItemComment?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.CommentNotFound);
        await _commentRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsNotOwner()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid commentId = Guid.NewGuid();
        Guid ownerId = Guid.NewGuid();
        Guid currentUserId = Guid.NewGuid();
        DeleteCommentCommand command = new(projectId, workItemId, commentId);

        _workItemRepositoryMock.ExistsInProjectAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(true);
        _userContextMock.UserId.Returns(currentUserId);

        WorkItemComment comment = CreateComment(ownerId);
        _commentRepositoryMock.GetByIdTrackedAsync(commentId, workItemId, Arg.Any<CancellationToken>())
            .Returns(comment);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.CommentNotOwner);
        _commentRepositoryMock.DidNotReceive().Remove(Arg.Any<WorkItemComment>());
        await _commentRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
