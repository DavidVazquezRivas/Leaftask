using BuildingBlocks.DrivenInfrastructure.Inbox;
using BuildingBlocks.Infrastructure.Events;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Modules.Notification.Application.Events;
using Modules.Notification.Domain.Entities;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.Domain.Entities.Approval.Permission;
using ApprovalAction = Modules.Notification.Domain.Entities.Approval.Action;

namespace Modules.Notification.DrivenInfrastructure.Persistence;

public sealed class NotificationsDbContext(
    DbContextOptions<NotificationsDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher,
    NotificationsModuleEventMapper eventMapper) : AppDbContext(options, domainEventsDispatcher, eventMapper)
{
    public DbSet<Domain.Entities.Notification.Notification> Notifications { get; set; }
    public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
    public DbSet<RequestComment> RequestComments { get; set; }
    public DbSet<ApprovalAction> Actions { get; set; }
    public DbSet<UserReadModel> UserReadModels { get; set; }
    public DbSet<OrganizationPermissionReadModel> OrganizationPermissionReadModels { get; set; }
    public DbSet<ProjectPermissionReadModel> ProjectPermissionReadModels { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema.Notification);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
