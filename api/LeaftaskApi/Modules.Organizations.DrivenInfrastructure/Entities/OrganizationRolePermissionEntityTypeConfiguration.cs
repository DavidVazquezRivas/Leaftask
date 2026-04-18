using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.DrivenInfrastructure.Entities;

internal sealed class OrganizationRolePermissionEntityTypeConfiguration : IEntityTypeConfiguration<OrganizationRolePermission>
{
    public void Configure(EntityTypeBuilder<OrganizationRolePermission> builder)
    {
        builder.ToTable("organization_role_permissions");

        builder.HasKey(rolePermission => rolePermission.Id);

        builder.Property(rolePermission => rolePermission.Level)
            .HasColumnName("level")
            .IsRequired();

        builder.Property(rolePermission => rolePermission.OrganizationRoleId)
            .HasColumnName("organization_role_id")
            .IsRequired();

        builder.Property(rolePermission => rolePermission.OrganizationPermissionId)
            .HasColumnName("organization_permission_id")
            .IsRequired();

        builder.HasIndex(rolePermission => new { rolePermission.OrganizationRoleId, rolePermission.OrganizationPermissionId })
            .IsUnique();

        builder.HasOne<OrganizationPermission>()
            .WithMany()
            .HasForeignKey(rolePermission => rolePermission.OrganizationPermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
