using BuildingBlocks.Integration;

namespace Modules.WorkItems.Integration;

public sealed record UsersMentionedInCommentIntegrationEvent(
    Guid WorkItemId,
    Guid CommentId,
    Guid AuthorId,
    IReadOnlyList<Guid> MentionedUserIds) : IIntegrationEvent;
