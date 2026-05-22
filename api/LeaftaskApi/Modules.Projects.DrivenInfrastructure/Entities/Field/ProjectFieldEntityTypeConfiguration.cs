using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Field;

namespace Modules.Projects.DrivenInfrastructure.Entities.Field;

internal sealed class ProjectFieldEntityTypeConfiguration : IEntityTypeConfiguration<ProjectField>
{
    public void Configure(EntityTypeBuilder<ProjectField> builder)
    {
        builder.ToTable("project_fields");

        builder.HasKey(projectField => projectField.Id);

        builder.Property(projectField => projectField.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(projectField => projectField.Default)
            .HasColumnName("default")
            .IsRequired();

        builder.Property(projectField => projectField.Optional)
            .HasColumnName("optional")
            .IsRequired();

        builder.HasOne(projectField => projectField.Project)
            .WithMany()
            .HasForeignKey("project_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(projectField => projectField.Field)
            .WithMany()
            .HasForeignKey("field_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
