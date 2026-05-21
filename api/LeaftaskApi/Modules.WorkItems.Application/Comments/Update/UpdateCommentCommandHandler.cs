using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Comments.Update;

public sealed class UpdateCommentCommandHandler(
    IWorkItemRepository workItemRepository,
    IAttachmentRepository attachmentRepository,
    ICommentRepository commentRepository,
    IUserContext userContext)
    : ICommandHandler<UpdateCommentCommand, Result<CommentDto>>
{
    public async Task<Result<CommentDto>> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
    {
        bool workItemExists = await workItemRepository.ExistsInProjectAsync(
            command.WorkItemId, command.ProjectId, cancellationToken);

        if (!workItemExists)
        {
            return Result.Failure<CommentDto>(WorkItemErrors.WorkItemNotFound);
        }

        WorkItemComment? comment = await commentRepository.GetByIdTrackedAsync(
            command.CommentId, command.WorkItemId, cancellationToken);

        if (comment is null)
        {
            return Result.Failure<CommentDto>(WorkItemErrors.CommentNotFound);
        }

        if (comment.User.Id != userContext.UserId)
        {
            return Result.Failure<CommentDto>(WorkItemErrors.CommentNotOwner);
        }

        if (command.Content is not null)
        {
            comment.Update(command.Content);
        }

        List<Attachment> currentAttachments = await attachmentRepository.GetByCommentIdTrackedAsync(
            command.CommentId, cancellationToken);

        List<Attachment> finalAttachments = currentAttachments;

        if (command.AttachmentIds is not null)
        {
            foreach (Attachment attachment in currentAttachments)
            {
                attachment.LinkToComment(null);
            }

            if (command.AttachmentIds.Count > 0)
            {
                List<Attachment> newAttachments = await attachmentRepository.GetByIdsTrackedAsync(
                    command.AttachmentIds, command.WorkItemId, cancellationToken);

                if (newAttachments.Count != command.AttachmentIds.Count)
                {
                    return Result.Failure<CommentDto>(WorkItemErrors.AttachmentNotFound);
                }

                foreach (Attachment attachment in newAttachments)
                {
                    attachment.LinkToComment(comment);
                }

                finalAttachments = newAttachments;
            }
            else
            {
                finalAttachments = [];
            }
        }

        await commentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(new CommentDto(
            comment.Id,
            new CommentAuthorDto(comment.User.Id, $"{comment.User.FirstName} {comment.User.LastName}".Trim()),
            comment.Content,
            comment.CreatedAt,
            finalAttachments.Select(a => new CommentAttachmentDto(a.Id, a.FileName, a.FileUrl)).ToList()));
    }
}
