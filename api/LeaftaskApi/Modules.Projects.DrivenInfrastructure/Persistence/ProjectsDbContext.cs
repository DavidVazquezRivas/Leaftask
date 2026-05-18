using BuildingBlocks.DrivenInfrastructure.Inbox;
using BuildingBlocks.Infrastructure.Events;
using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Modules.Projects.Application.Events;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Field;
using Modules.Projects.Domain.Entities.Invitation;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.DrivenInfrastructure.Persistence;

public sealed class ProjectsDbContext(
    DbContextOptions<ProjectsDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher,
    ProjectModuleEventMapper eventMapper) : AppDbContext(options, domainEventsDispatcher, eventMapper)
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<ProjectInvitation> ProjectInvitations { get; set; }
    public DbSet<ProjectRole> ProjectRoles { get; set; }
    public DbSet<ProjectPermissionGroup> ProjectPermissionGroups { get; set; }
    public DbSet<ProjectPermission> ProjectPermissions { get; set; }
    public DbSet<ProjectRolePermission> ProjectRolePermissions { get; set; }
    public DbSet<FieldType> FieldTypes { get; set; }
    public DbSet<Field> Fields { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<ProjectField> ProjectFields { get; set; }
    public DbSet<WorkItemTypeReadModel> WorkItemTypeReadModels { get; set; }
    public DbSet<UserReadModel> UserReadModels { get; set; }
    public DbSet<OrganizationReadModel> OrganizationReadModels { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema.Project);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectsDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
