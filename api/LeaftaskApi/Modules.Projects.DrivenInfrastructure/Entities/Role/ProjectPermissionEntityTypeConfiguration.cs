using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.DrivenInfrastructure.Entities.Role;

internal sealed class ProjectPermissionEntityTypeConfiguration : IEntityTypeConfiguration<ProjectPermission>
{
    public void Configure(EntityTypeBuilder<ProjectPermission> builder)
    {
        builder.ToTable("project_permissions");

        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(permission => permission.Description)
            .HasColumnName("description")
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne(permission => permission.PermissionGroup)
            .WithMany()
            .HasForeignKey("permission_group_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(permission => permission.Name)
            .IsUnique();
    }
}
