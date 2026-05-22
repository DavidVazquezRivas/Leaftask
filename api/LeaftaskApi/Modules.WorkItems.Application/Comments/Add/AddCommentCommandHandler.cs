using System.Text.RegularExpressions;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Comments.Add;

public sealed class AddCommentCommandHandler(
    IWorkItemRepository workItemRepository,
    IUserReadModelRepository userReadModelRepository,
    IAttachmentRepository attachmentRepository,
    ICommentRepository commentRepository,
    IUserContext userContext)
    : ICommandHandler<AddCommentCommand, Result<CommentDto>>
{
    private static readonly Regex MentionRegex = new(
        @"data-id=""([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})""",
        RegexOptions.Compiled);

    public async Task<Result<CommentDto>> Handle(AddCommentCommand command, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await workItemRepository.GetByIdTrackedAsync(
            command.WorkItemId, command.ProjectId, cancellationToken);

        if (workItem is null)
        {
            return Result.Failure<CommentDto>(WorkItemErrors.WorkItemNotFound);
        }

        UserReadModel? user = await userReadModelRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<CommentDto>(WorkItemErrors.AssigneeNotFound);
        }

        List<Attachment> attachments = [];
        if (command.AttachmentIds.Count > 0)
        {
            attachments = await attachmentRepository.GetByIdsTrackedAsync(
                command.AttachmentIds, command.WorkItemId, cancellationToken);

            if (attachments.Count != command.AttachmentIds.Count)
            {
                return Result.Failure<CommentDto>(WorkItemErrors.AttachmentNotFound);
            }
        }

        IReadOnlyList<Guid> mentionedUserIds = ExtractMentionedUserIds(command.Content);
        WorkItemComment comment = workItem.AddComment(command.Content, user, mentionedUserIds);

        foreach (Attachment attachment in attachments)
        {
            attachment.LinkToComment(comment);
        }

        await commentRepository.AddAsync(comment, cancellationToken);
        await commentRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(new CommentDto(
            comment.Id,
            new CommentAuthorDto(user.Id, $"{user.FirstName} {user.LastName}".Trim()),
            comment.Content,
            comment.CreatedAt,
            attachments.Select(a => new CommentAttachmentDto(a.Id, a.FileName, a.FileUrl)).ToList()));
    }

    private static List<Guid> ExtractMentionedUserIds(string content)
    {
        return MentionRegex.Matches(content)
            .Select(m => Guid.Parse(m.Groups[1].Value))
            .Distinct()
            .ToList();
    }
}
