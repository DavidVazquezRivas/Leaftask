using BuildingBlocks.Domain.Events;

namespace Modules.WorkItems.Domain.Events;

public sealed record CommentAddedDomainEvent(
    Guid WorkItemId,
    Guid CommentId,
    Guid AuthorId,
    IReadOnlyList<Guid> MentionedUserIds) : IDomainEvent;
