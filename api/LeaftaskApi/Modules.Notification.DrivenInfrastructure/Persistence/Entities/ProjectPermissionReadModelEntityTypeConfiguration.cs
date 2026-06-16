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
    }
}
