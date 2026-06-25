using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.WorkItems.Application.Attachments;
using Modules.WorkItems.Application.Attachments.GetPresignedUrl;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Attachments.GetPresignedUrl;

public class GetAttachmentPresignedUrlQueryHandlerTests
{
    private readonly GetAttachmentPresignedUrlQueryHandler _handler;
    private readonly IWorkItemRepository _workItemRepositoryMock;
    private readonly IFileStorage _fileStorageMock;

    public GetAttachmentPresignedUrlQueryHandlerTests()
    {
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();
        _fileStorageMock = Substitute.For<IFileStorage>();
        _handler = new GetAttachmentPresignedUrlQueryHandler(_workItemRepositoryMock, _fileStorageMock);
    }

    private static WorkItem CreateWorkItem(Guid projectId)
    {
        ProjectReadModel project = new(projectId, "TST");
        WorkItemStatus status = new(Guid.NewGuid(), "To Do");
        WorkItemType type = new(Guid.NewGuid(), "Task");
        return WorkItem.Create(1, "Title", "Desc", 3m, DateTime.UtcNow.AddDays(7), project, status, type);
    }

    [Fact]
    public async Task Handle_Should_ReturnPresignedUrl_When_WorkItemExists()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        WorkItem workItem = CreateWorkItem(projectId);
        Uri presigned = new("https://storage.example.com/upload?sig=abc");
        Uri publicUri = new("https://storage.example.com/file.png");

        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns(workItem);
        _fileStorageMock.GetPresignedPutUrlAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(new PresignedUploadResult(presigned, publicUri));

        GetAttachmentPresignedUrlQuery query = new(projectId, workItemId, "file.png");

        // Act
        Result<PresignedUrlDto> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.PresignedUrl.Should().Be(presigned);
        result.Value.PublicUrl.Should().Be(publicUri);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_WorkItemNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid workItemId = Guid.NewGuid();
        _workItemRepositoryMock.GetByIdTrackedAsync(workItemId, projectId, Arg.Any<CancellationToken>())
            .Returns((WorkItem?)null);

        GetAttachmentPresignedUrlQuery query = new(projectId, workItemId, "file.png");

        // Act
        Result<PresignedUrlDto> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(WorkItemErrors.WorkItemNotFound);
    }
}
