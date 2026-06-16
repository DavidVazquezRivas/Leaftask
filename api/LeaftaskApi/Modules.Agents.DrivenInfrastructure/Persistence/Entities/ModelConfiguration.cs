using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Agents.Domain.Entities.Model;

namespace Modules.Agents.DrivenInfrastructure.Persistence.Entities;

public sealed class ModelConfiguration : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        builder.ToTable("models");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");
        builder.Property(m => m.Name).HasColumnName("name").IsRequired();
        builder.Property(m => m.Description).HasColumnName("description").IsRequired();
        builder.Property(m => m.Cost).HasColumnName("cost");

        builder.HasOne(m => m.Provider)
            .WithMany()
            .HasForeignKey("provider_id")
            .IsRequired();
    }
}
