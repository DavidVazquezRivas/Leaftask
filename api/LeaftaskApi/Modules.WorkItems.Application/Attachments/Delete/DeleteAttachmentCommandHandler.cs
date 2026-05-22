using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Attachments.Delete;

public sealed class DeleteAttachmentCommandHandler(
    IWorkItemRepository workItemRepository,
    IAttachmentRepository attachmentRepository,
    IFileStorage fileStorage,
    IUserContext userContext)
    : ICommandHandler<DeleteAttachmentCommand, Result>
{
    public async Task<Result> Handle(DeleteAttachmentCommand command, CancellationToken cancellationToken)
    {
        bool workItemExists = await workItemRepository.ExistsInProjectAsync(
            command.WorkItemId, command.ProjectId, cancellationToken);

        if (!workItemExists)
        {
            return Result.Failure(WorkItemErrors.WorkItemNotFound);
        }

        Attachment? attachment = await attachmentRepository.GetByIdTrackedAsync(
            command.AttachmentId, command.WorkItemId, cancellationToken);

        if (attachment is null)
        {
            return Result.Failure(WorkItemErrors.AttachmentNotFound);
        }

        if (attachment.User.Id != userContext.UserId)
        {
            return Result.Failure(WorkItemErrors.AttachmentNotOwner);
        }

        string objectKey = $"workitems/{command.WorkItemId}/{command.AttachmentId}/{attachment.FileName}";
        await fileStorage.DeleteAsync(objectKey, cancellationToken);

        attachmentRepository.Remove(attachment);
        await attachmentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
