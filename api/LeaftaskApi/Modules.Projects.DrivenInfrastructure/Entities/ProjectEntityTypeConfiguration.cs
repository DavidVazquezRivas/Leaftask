using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;

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

        builder.Property(project => project.Owner)
            .HasColumnName("owner_id")
            .HasConversion(
                owner => owner.Id,
                id => new ProjectOwnerReference(id))
            .Metadata.SetValueComparer(new ValueComparer<IProjectOwner>(
                (left, right) => left != null && right != null && left.Id == right.Id,
                owner => owner.Id.GetHashCode(),
                owner => new ProjectOwnerReference(owner.Id)));

        builder.Property(project => project.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex("owner_id", "owner_type");
        builder.HasIndex(project => project.Abbreviation).IsUnique();
    }
}
