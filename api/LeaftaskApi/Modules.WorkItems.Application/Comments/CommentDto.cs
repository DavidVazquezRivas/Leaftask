namespace Modules.WorkItems.Application.Comments;

public sealed record CommentDto(
    Guid Id,
    CommentAuthorDto Author,
    string Content,
    DateTime CreatedAt,
    IReadOnlyList<CommentAttachmentDto> Attachments);

public sealed record CommentAuthorDto(Guid Id, string FullName);

public sealed record CommentAttachmentDto(Guid Id, string FileName, Uri Url);
