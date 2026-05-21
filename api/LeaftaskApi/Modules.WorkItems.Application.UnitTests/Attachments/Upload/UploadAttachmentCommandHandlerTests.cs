using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.Attachments;
using Modules.WorkItems.Application.Attachments.Upload;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Attachments.Upload;

public class UploadAttachmentCommandHandlerTests
{
    private readonly UploadAttachmentCommandHandler _handler;
    private readonly IWorkItemRepository _workItemRepositoryMock;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;
    private readonly IAttachmentRepository _attachmentRepositoryMock;
    private readonly IFileStorage _fileStorageMock;
    private readonly IUserContext _userContextMock;

    public UploadAttachmentCommandHandlerTests()
    {
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();
        _attachmentRepositoryMock = Substitute.For<IAttachmentRepository>();
        _fileStorageMock = Substitute.For<IFileStorage>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new UploadAttachmentCommandHandler(
            _workItemRepositoryMock,
            _userReadModelRepositoryMock,
            _attachmentRepositoryMock,
            _fileStorageMock,
            _userContextMock);
    }

    private static WorkItem CreateWorkItem()
    {
        ProjectReadModel project = new(Guid.NewGuid(), "TST");
        WorkItemStatus status = new(Guid.NewGuid(), "To Do");
        WorkItemType type = new(Guid.NewGuid(), "Task");
        return WorkItem.Create(1, "Title", "Desc", 3m, DateTime.UtcNow.AddDays(7), project, status, type);
    }

    [Fact]
    public async Task Handle_Should_UploadFile_And_ReturnAttachmentDto()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        MemoryStream stream = new([1, 2, 3]);
        UploadAttachmentCommand command = new(projectId, workItemId, stream, "doc.pdf", "application/pdf", 3L);

        WorkItem workItem = CreateWorkItem();
        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(workItem);

        _userContextMock.UserId.Returns(userId);
        UserReadModel user = new(userId, "Alice", "Smith");
        _userReadModelRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        Uri fileUrl = new("http://localhost:9000/bucket/workitems/obj/doc.pdf");
        _fileStorageMock
            .UploadAsync(Arg.Any<string>(), stream, "application/pdf", 3L, Arg.Any<CancellationToken>())
            .Returns(fileUrl);

        // Act
        Result<AttachmentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.FileName.Should().Be("doc.pdf");
        result.Value.Url.Should().Be(fileUrl);
        await _attachmentRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        MemoryStream stream = new([]);
        UploadAttachmentCommand command = new(projectId, workItemId, stream, "doc.pdf", "application/pdf", 0L);

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns((WorkItem?)null);

        // Act
        Result<AttachmentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);
        await _fileStorageMock.DidNotReceive()
            .UploadAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<long>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        MemoryStream stream = new([]);
        UploadAttachmentCommand command = new(projectId, workItemId, stream, "doc.pdf", "application/pdf", 0L);

        WorkItem workItem = CreateWorkItem();
        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _userContextMock.UserId.Returns(userId);
        _userReadModelRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((UserReadModel?)null);

        // Act
        Result<AttachmentDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.AssigneeNotFound);
        await _fileStorageMock.DidNotReceive()
            .UploadAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<long>(), Arg.Any<CancellationToken>());
    }
}
