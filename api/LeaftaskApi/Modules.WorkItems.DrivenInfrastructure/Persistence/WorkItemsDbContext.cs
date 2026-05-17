using BuildingBlocks.DrivenInfrastructure.Inbox;
using BuildingBlocks.Infrastructure.Events;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Application.Events;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Field;
using Modules.WorkItems.Domain.Entities.Properties;

namespace Modules.WorkItems.DrivenInfrastructure.Persistence;

public sealed class WorkItemsDbContext(
    DbContextOptions<WorkItemsDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher,
    WorkItemsModuleEventMapper eventMapper) : AppDbContext(options, domainEventsDispatcher, eventMapper)
{
    public DbSet<WorkItem> WorkItems { get; set; }
    public DbSet<WorkItemComment> WorkItemComments { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<WorkLog> WorkLogs { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<WorkItemStatus> WorkItemStatuses { get; set; }
    public DbSet<WorkItemType> WorkItemTypes { get; set; }
    public DbSet<ProjectReadModel> ProjectReadModels { get; set; }
    public DbSet<UserReadModel> UserReadModels { get; set; }
    public DbSet<FieldTypeReadModel> FieldTypeReadModels { get; set; }
    public DbSet<FieldReadModel> FieldReadModels { get; set; }
    public DbSet<FieldValue> FieldValues { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema.WorkItem);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkItemsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
