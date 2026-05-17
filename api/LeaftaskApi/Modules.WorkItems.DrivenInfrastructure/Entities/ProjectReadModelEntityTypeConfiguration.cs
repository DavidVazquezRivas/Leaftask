using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.WorkItems.Domain.Entities;

namespace Modules.WorkItems.DrivenInfrastructure.Entities;

internal sealed class ProjectReadModelEntityTypeConfiguration : IEntityTypeConfiguration<ProjectReadModel>
{
    public void Configure(EntityTypeBuilder<ProjectReadModel> builder)
    {
        builder.ToTable("project_read_models");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Abbreviation)
            .HasColumnName("abbreviation")
            .HasMaxLength(20)
            .IsRequired();
    }
}
