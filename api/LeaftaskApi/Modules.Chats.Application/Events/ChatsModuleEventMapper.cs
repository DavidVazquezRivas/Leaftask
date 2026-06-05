using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using Modules.Chats.Domain.Events;
using Modules.Chats.Integration;

namespace Modules.Chats.Application.Events;

public sealed class ChatsModuleEventMapper : IIntegrationEventMapper
{
    public IIntegrationEvent? Map(IDomainEvent domainEvent) =>
        domainEvent switch
        {
            MessageCreatedDomainEvent e =>
                new ChatMessageSentIntegrationEvent(e.ChatId, e.MessageId, e.SenderId, e.Content),
            _ => null
        };
}
