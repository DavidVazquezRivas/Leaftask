using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.DrivenInfrastructure.Entities.Role;

internal sealed class ProjectPermissionGroupEntityTypeConfiguration : IEntityTypeConfiguration<ProjectPermissionGroup>
{
    public void Configure(EntityTypeBuilder<ProjectPermissionGroup> builder)
    {
        builder.ToTable("project_permission_groups");

        builder.HasKey(group => group.Id);

        builder.Property(group => group.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(group => group.Name)
            .IsUnique();
    }
}
