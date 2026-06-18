using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Notification.Domain.Entities.Approval.Permission;

namespace Modules.Notification.DrivenInfrastructure.Persistence.Entities;

internal sealed class ProjectPermissionReadModelEntityTypeConfiguration
    : IEntityTypeConfiguration<ProjectPermissionReadModel>
{
    public void Configure(EntityTypeBuilder<ProjectPermissionReadModel> builder)
    {
        builder.ToTable("project_permission_read_models");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(p => p.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(p => p.PermissionName).HasColumnName("permission_name").IsRequired();
        builder.Property(p => p.Level).HasColumnName("level").IsRequired();
    }
}
