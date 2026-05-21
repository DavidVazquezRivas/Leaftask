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
            CommentAddedDomainEvent e when e.MentionedUserIds.Count > 0 =>
                new UsersMentionedInCommentIntegrationEvent(
                    e.WorkItemId,
                    e.CommentId,
                    e.AuthorId,
                    e.MentionedUserIds),
            _ => null
        };
}
