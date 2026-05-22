using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.WorkItems.Domain.Entities.Field;

namespace Modules.WorkItems.DrivenInfrastructure.Entities.Field;

internal sealed class FieldValueEntityTypeConfiguration : IEntityTypeConfiguration<FieldValue>
{
    public void Configure(EntityTypeBuilder<FieldValue> builder)
    {
        builder.ToTable("field_values");

        builder.HasKey(fv => fv.Id);

        builder.Property(fv => fv.Value)
            .HasColumnName("value")
            .IsRequired();

        builder.HasOne(fv => fv.Field)
            .WithMany()
            .HasForeignKey("field_read_model_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fv => fv.WorkItem)
            .WithMany()
            .HasForeignKey("work_item_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("field_read_model_id", "work_item_id").IsUnique();
    }
}
