using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.WorkItems.Domain.Entities;

namespace Modules.WorkItems.DrivenInfrastructure.Entities;

internal sealed class ActivityLogEntityTypeConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("activity_logs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FieldName)
            .HasColumnName("field_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.OldValue)
            .HasColumnName("old_value")
            .IsRequired();

        builder.Property(a => a.NewValue)
            .HasColumnName("new_value")
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne(a => a.WorkItem)
            .WithMany()
            .HasForeignKey("work_item_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.UserReadModel)
            .WithMany()
            .HasForeignKey("user_read_model_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
