using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using Modules.WorkItems.Domain.Events;
using Modules.WorkItems.Integration;

namespace Modules.WorkItems.Application.Events;

public sealed class WorkItemsModuleEventMapper : IIntegrationEventMapper
{
    public IIntegrationEvent? Map(IDomainEvent domainEvent) =>
        domainEvent switch
        {
            WorkItemCreatedDomainEvent e =>
                new WorkItemCreatedIntegrationEvent(e.WorkItemId, e.ProjectId, e.Title, e.StatusId, e.TypeId, e.AssigneeId),

            WorkItemStatusChangedDomainEvent e =>
                new WorkItemStatusChangedIntegrationEvent(e.WorkItemId, e.ProjectId, e.OldStatusId, e.NewStatusId),

            WorkItemAssigneeChangedDomainEvent e =>
                new WorkItemAssigneeChangedIntegrationEvent(e.WorkItemId, e.ProjectId, e.OldAssigneeId, e.NewAssigneeId),

            WorkItemDeletedDomainEvent e =>
                new WorkItemDeletedIntegrationEvent(e.WorkItemId, e.ProjectId),

            WorkItemProgressUpdatedDomainEvent e =>
                new WorkItemProgressUpdatedIntegrationEvent(e.WorkItemId, e.ProjectId, e.OldProgress, e.NewProgress),

            WorkItemCommentAddedDomainEvent e =>
                new WorkItemCommentAddedIntegrationEvent(e.WorkItemId, e.ProjectId, e.CommentId, e.AuthorId),

            CommentAddedDomainEvent e when e.MentionedUserIds.Count > 0 =>
                new UsersMentionedInCommentIntegrationEvent(e.WorkItemId, e.CommentId, e.AuthorId, e.MentionedUserIds),

            _ => null
        };
}
