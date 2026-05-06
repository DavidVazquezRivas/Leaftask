using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.DrivenInfrastructure.Entities.Role;

internal sealed class ProjectRoleEntityTypeConfiguration : IEntityTypeConfiguration<ProjectRole>
{
    public void Configure(EntityTypeBuilder<ProjectRole> builder)
    {
        builder.ToTable("project_roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(role => role.Project)
            .WithMany()
            .HasForeignKey("project_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("project_id", nameof(ProjectRole.Name))
            .IsUnique();
    }
}
