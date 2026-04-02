using BuildingBlocks.Domain.Entities;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Infrastructure.Events;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence;

public abstract class AppDbContext : DbContext
{
    private readonly IDomainEventsDispatcher _domainEventsDispatcher;

    protected AppDbContext(DbContextOptions options, IDomainEventsDispatcher domainEventsDispatcher)
        : base(options) => _domainEventsDispatcher = domainEventsDispatcher;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await PublishDomainEventsAsync();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task PublishDomainEventsAsync()
    {
        List<IDomainEvent> domainEvents = ChangeTracker
            .Entries<Entity>()
            .SelectMany(e =>
            {
                IReadOnlyCollection<IDomainEvent> events = e.Entity.DomainEvents.ToList();
                e.Entity.ClearDomainEvents();
                return events;
            })
            .ToList();

        await _domainEventsDispatcher.DispatchAsync(domainEvents);
    }
}
