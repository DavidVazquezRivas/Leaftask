using System.Text.Json;
using BuildingBlocks.DrivenInfrastructure.Inbox;
using BuildingBlocks.Integration;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.DrivingInfrastructure.Events;

public abstract class IntegrationEventHandler<TIntegrationEvent, TContext>(
    TContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor)
    : INotificationHandler<TIntegrationEvent>
    where TIntegrationEvent : class, IIntegrationEvent
    where TContext : DbContext
{
    public async Task Handle(TIntegrationEvent notification, CancellationToken cancellationToken)
    {
        Guid messageId = integrationEventContextAccessor.CurrentMessageId ?? GetFallbackMessageId(notification);

        bool alreadyProcessed = await dbContext.Set<InboxMessage>()
            .AsNoTracking()
            .AnyAsync(message => message.Id == messageId, cancellationToken);

        if (alreadyProcessed)
        {
            return;
        }

        await HandleIntegrationEvent(notification, cancellationToken);

        string type = notification.GetType().AssemblyQualifiedName!;
        string content = JsonSerializer.Serialize(notification, notification.GetType());

        dbContext.Set<InboxMessage>().Add(InboxMessage.Create(messageId, type, content));

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    protected abstract Task HandleIntegrationEvent(TIntegrationEvent notification, CancellationToken cancellationToken);

    protected abstract Guid GetFallbackMessageId(TIntegrationEvent notification);
}
