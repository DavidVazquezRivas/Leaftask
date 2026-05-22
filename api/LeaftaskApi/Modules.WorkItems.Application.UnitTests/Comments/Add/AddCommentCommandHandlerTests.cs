using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.Comments;
using Modules.WorkItems.Application.Comments.Add;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Comments.Add;

public class AddCommentCommandHandlerTests
{
    private readonly AddCommentCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepositoryMock;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;
    private readonly IAttachmentRepository _attachmentRepositoryMock;
    private readonly ICommentRepository _commentRepositoryMock;
    private readonly IUserContext _userContextMock;

    public AddCommentCommandHandlerTests()
    {
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();
        _attachmentRepositoryMock = Substitute.For<IAttachmentRepository>();
        _commentRepositoryMock = Substitute.For<ICommentRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new AddCommentCommandHandler(
            _workItemRepositoryMock,
            _userReadModelRepositoryMock,
            _attachmentRepositoryMock,
            _commentRepositoryMock,
            _userContextMock);
    }

    private static WorkItem CreateWorkItem()
    {
        ProjectReadModel project = new(Guid.NewGuid(), "TST");
        WorkItemStatus status = new(Guid.NewGuid(), "To Do");
        WorkItemType type = new(Guid.NewGuid(), "Task");
        return WorkItem.Create(1, "Title", "Desc", 0m, DateTime.UtcNow.AddDays(7), project, status, type);
    }

    [Fact]
    public async Task Handle_Should_ReturnCommentDto_When_WorkItemExistsAndNoAttachments()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        AddCommentCommand command = new(projectId, workItemId, "Hello world", []);

        WorkItem workItem = CreateWorkItem();
        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(workItem);

        UserReadModel user = new(userId, "Alice", "Smith");
        _userContextMock.UserId.Returns(userId);
        _userReadModelRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        Result<CommentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Content.Should().Be("Hello world");
        result.Value.Author.Id.Should().Be(userId);
        result.Value.Attachments.Should().BeEmpty();
        await _commentRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        AddCommentCommand command = new(projectId, workItemId, "Content", []);

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns((WorkItem?)null);

        // Act
        Result<CommentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);
        await _commentRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        AddCommentCommand command = new(projectId, workItemId, "Content", []);

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(CreateWorkItem());

        _userContextMock.UserId.Returns(userId);
        _userReadModelRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((UserReadModel?)null);

        // Act
        Result<CommentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.AssigneeNotFound);
        await _commentRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AttachmentNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Guid missingAttachmentId = Guid.NewGuid();
        AddCommentCommand command = new(projectId, workItemId, "Content", [missingAttachmentId]);

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(CreateWorkItem());

        UserReadModel user = new(userId, "Alice", "Smith");
        _userContextMock.UserId.Returns(userId);
        _userReadModelRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _attachmentRepositoryMock
            .GetByIdsTrackedAsync(Arg.Any<IReadOnlyList<Guid>>(), workItemId, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result<CommentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.AttachmentNotFound);
        await _commentRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
