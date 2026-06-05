using BuildingBlocks.Domain.Events;

namespace Modules.WorkItems.Domain.Events;

public sealed record WorkItemCommentAddedDomainEvent(
    Guid WorkItemId,
    Guid ProjectId,
    Guid CommentId,
    Guid AuthorId) : IDomainEvent;
