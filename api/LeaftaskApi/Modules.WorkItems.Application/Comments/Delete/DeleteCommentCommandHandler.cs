using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Comments.Delete;

public sealed class DeleteCommentCommandHandler(
    IWorkItemRepository workItemRepository,
    IAttachmentRepository attachmentRepository,
    ICommentRepository commentRepository,
    IUserContext userContext)
    : ICommandHandler<DeleteCommentCommand, Result>
{
    public async Task<Result> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        bool workItemExists = await workItemRepository.ExistsInProjectAsync(
            command.WorkItemId, command.ProjectId, cancellationToken);

        if (!workItemExists)
        {
            return Result.Failure(WorkItemErrors.WorkItemNotFound);
        }

        WorkItemComment? comment = await commentRepository.GetByIdTrackedAsync(
            command.CommentId, command.WorkItemId, cancellationToken);

        if (comment is null)
        {
            return Result.Failure(WorkItemErrors.CommentNotFound);
        }

        if (comment.User.Id != userContext.UserId)
        {
            return Result.Failure(WorkItemErrors.CommentNotOwner);
        }

        List<Attachment> attachments = await attachmentRepository.GetByCommentIdTrackedAsync(
            command.CommentId, cancellationToken);

        foreach (Attachment attachment in attachments)
        {
            attachment.LinkToComment(null);
        }

        commentRepository.Remove(comment);
        await commentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
