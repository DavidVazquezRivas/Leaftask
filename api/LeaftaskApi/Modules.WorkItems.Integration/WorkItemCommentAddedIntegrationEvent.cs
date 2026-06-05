using BuildingBlocks.Integration;

namespace Modules.WorkItems.Integration;

public sealed record WorkItemCommentAddedIntegrationEvent(
    Guid WorkItemId,
    Guid ProjectId,
    Guid CommentId,
    Guid AuthorId) : IIntegrationEvent;
