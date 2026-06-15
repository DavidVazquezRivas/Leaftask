using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Notification.Domain.Entities.Approval.Permission;

namespace Modules.Notification.DrivenInfrastructure.Persistence.Entities;

internal sealed class OrganizationPermissionReadModelEntityTypeConfiguration
    : IEntityTypeConfiguration<OrganizationPermissionReadModel>
{
    public void Configure(EntityTypeBuilder<OrganizationPermissionReadModel> builder)
    {
        builder.ToTable("organization_permission_read_models");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(p => p.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Property(p => p.PermissionName)
            .HasColumnName("permission_name")
            .IsRequired();

        builder.Property(p => p.Level)
            .HasColumnName("level")
            .IsRequired();
    }
}
