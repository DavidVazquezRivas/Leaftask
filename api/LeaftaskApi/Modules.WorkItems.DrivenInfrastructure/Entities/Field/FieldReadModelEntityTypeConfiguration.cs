using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.WorkItems.Domain.Entities.Field;
using Modules.WorkItems.Domain.Entities.Properties;

namespace Modules.WorkItems.DrivenInfrastructure.Entities.Field;

internal sealed class FieldReadModelEntityTypeConfiguration : IEntityTypeConfiguration<FieldReadModel>
{
    public void Configure(EntityTypeBuilder<FieldReadModel> builder)
    {
        builder.ToTable("field_read_models");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(f => f.IsOptional)
            .HasColumnName("is_optional")
            .IsRequired();

        builder.HasOne(f => f.FieldType)
            .WithMany()
            .HasForeignKey("field_type_read_model_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.AppliesTo)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "FieldReadModelWorkItemType",
                r => r.HasOne<WorkItemType>().WithMany().HasForeignKey("work_item_type_id").OnDelete(DeleteBehavior.Cascade),
                l => l.HasOne<FieldReadModel>().WithMany().HasForeignKey("field_read_model_id").OnDelete(DeleteBehavior.Cascade),
                j => j.ToTable("field_read_model_work_item_types"));
    }
}
