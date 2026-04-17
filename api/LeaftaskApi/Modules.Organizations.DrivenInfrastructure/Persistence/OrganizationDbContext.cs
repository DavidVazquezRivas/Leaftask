using BuildingBlocks.DrivenInfrastructure.Inbox;
using BuildingBlocks.Infrastructure.Events;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Application.Events;
using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.DrivenInfrastructure.Persistence;

public sealed class OrganizationDbContext(
    DbContextOptions<OrganizationDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher,
    OrganizationModuleEventMapper mapper) : AppDbContext(options, domainEventsDispatcher, mapper)
{
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<UserReadModel> UserReadModels { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema.Organization);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganizationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
