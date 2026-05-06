using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.DrivenInfrastructure.Entities.Role;

internal sealed class ProjectRolePermissionEntityTypeConfiguration : IEntityTypeConfiguration<ProjectRolePermission>
{
    public void Configure(EntityTypeBuilder<ProjectRolePermission> builder)
    {
        builder.ToTable("project_role_permissions");

        builder.HasKey(rolePermission => rolePermission.Id);

        builder.Property(rolePermission => rolePermission.PermissionLevel)
            .HasColumnName("permission_level")
            .IsRequired();

        builder.HasOne(rolePermission => rolePermission.Role)
            .WithMany()
            .HasForeignKey("project_role_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rolePermission => rolePermission.Permission)
            .WithMany()
            .HasForeignKey("project_permission_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("project_role_id", "project_permission_id")
            .IsUnique();
    }
}
