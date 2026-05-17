using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.WorkItems.Domain.Entities;

namespace Modules.WorkItems.DrivenInfrastructure.Entities;

internal sealed class WorkLogEntityTypeConfiguration : IEntityTypeConfiguration<WorkLog>
{
    public void Configure(EntityTypeBuilder<WorkLog> builder)
    {
        builder.ToTable("work_logs");

        builder.HasKey(wl => wl.Id);

        builder.Property(wl => wl.Date)
            .HasColumnName("date")
            .IsRequired();

        builder.Property(wl => wl.Hours)
            .HasColumnName("hours")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(wl => wl.Comment)
            .HasColumnName("comment")
            .IsRequired();

        builder.HasOne(wl => wl.WorkItem)
            .WithMany()
            .HasForeignKey("work_item_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wl => wl.User)
            .WithMany()
            .HasForeignKey("user_read_model_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
