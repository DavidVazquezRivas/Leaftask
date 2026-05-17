using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.WorkItems.Domain.Entities;

namespace Modules.WorkItems.DrivenInfrastructure.Entities;

internal sealed class WorkItemEntityTypeConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.ToTable("work_items");

        builder.HasKey(wi => wi.Id);

        builder.Property(wi => wi.Code)
            .HasColumnName("code")
            .IsRequired();

        builder.Property(wi => wi.Title)
            .HasColumnName("title")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(wi => wi.Description)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(wi => wi.Progress)
            .HasColumnName("progress")
            .IsRequired();

        builder.Property(wi => wi.LimitDate)
            .HasColumnName("limit_date")
            .IsRequired();

        builder.HasOne(wi => wi.Project)
            .WithMany()
            .HasForeignKey("project_read_model_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wi => wi.Status)
            .WithMany()
            .HasForeignKey("status_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wi => wi.Type)
            .WithMany()
            .HasForeignKey("type_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(wi => wi.Asignee)
            .WithMany()
            .HasForeignKey("assignee_id")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
