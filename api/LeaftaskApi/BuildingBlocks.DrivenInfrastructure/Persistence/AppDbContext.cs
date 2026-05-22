using System.Text.Json;
using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.DrivenInfrastructure.Outbox;
using BuildingBlocks.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence;

public abstract class AppDbContext(
    DbContextOptions options,
    IDomainEventsDispatcher domainEventsDispatcher,
    IIntegrationEventMapper eventMapper)
    : DbContext(options)
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        List<IDomainEvent> domainEvents = GetDomainEvents();

        await domainEventsDispatcher.DispatchAsync(domainEvents, cancellationToken);

        InsertOutboxMessages(domainEvents);

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void InsertOutboxMessages(List<IDomainEvent> domainEvents)
    {
        List<OutboxMessage> outboxMessages = domainEvents
            .Select(domainEvent => eventMapper.Map(domainEvent))
            .Where(integrationEvent => integrationEvent is not null)
            .Select(integrationEvent => OutboxMessage.Create(
                integrationEvent!.GetType().AssemblyQualifiedName!,
                JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType())))
            .ToList();

        OutboxMessages.AddRange(outboxMessages);
    }

    private List<IDomainEvent> GetDomainEvents() =>
        ChangeTracker
            .Entries<Entity>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
}
