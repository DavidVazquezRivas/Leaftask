using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities;

namespace Modules.Projects.DrivenInfrastructure.Entities;

internal sealed class ProjectEntityTypeConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(project => project.Abbreviation)
            .HasColumnName("abbreviation")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(project => project.Privacy)
            .HasColumnName("privacy")
            .IsRequired();

        builder.Property(project => project.OwnerType)
            .HasColumnName("owner_type")
            .IsRequired();

        builder.Property(project => project.OwnerId)
            .HasColumnName("owner_id")
            .IsRequired();

        builder.Ignore(project => project.Owner);

        builder.Property(project => project.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(nameof(Project.OwnerId), nameof(Project.OwnerType));
        builder.HasIndex(nameof(Project.OwnerId), nameof(Project.Abbreviation)).IsUnique();
    }
}
