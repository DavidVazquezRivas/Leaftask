using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Attachments.Upload;

public sealed class UploadAttachmentCommandHandler(
    IWorkItemRepository workItemRepository,
    IUserReadModelRepository userReadModelRepository,
    IAttachmentRepository attachmentRepository,
    IFileStorage fileStorage,
    IUserContext userContext)
    : ICommandHandler<UploadAttachmentCommand, Result<AttachmentDto>>
{
    public async Task<Result<AttachmentDto>> Handle(UploadAttachmentCommand command, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await workItemRepository.GetByIdTrackedAsync(
            command.WorkItemId, command.ProjectId, cancellationToken);

        if (workItem is null)
        {
            return Result.Failure<AttachmentDto>(WorkItemErrors.WorkItemNotFound);
        }

        UserReadModel? user = await userReadModelRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<AttachmentDto>(WorkItemErrors.AssigneeNotFound);
        }

        Guid attachmentId = Guid.NewGuid();
        string objectKey = $"workitems/{command.WorkItemId}/{attachmentId}/{command.FileName}";

        Uri fileUrl = await fileStorage.UploadAsync(
            objectKey, command.FileContent, command.ContentType, command.FileSize, cancellationToken);

        Attachment attachment = new(attachmentId, command.FileName, fileUrl, DateTime.UtcNow, workItem, user);
        await attachmentRepository.AddAsync(attachment, cancellationToken);
        await attachmentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(new AttachmentDto(attachment.Id, attachment.FileName, attachment.FileUrl));
    }
}
