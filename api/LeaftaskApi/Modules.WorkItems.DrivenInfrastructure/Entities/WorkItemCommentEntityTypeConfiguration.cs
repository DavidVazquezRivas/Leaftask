using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.WorkItems.Domain.Entities;

namespace Modules.WorkItems.DrivenInfrastructure.Entities;

internal sealed class WorkItemCommentEntityTypeConfiguration : IEntityTypeConfiguration<WorkItemComment>
{
    public void Configure(EntityTypeBuilder<WorkItemComment> builder)
    {
        builder.ToTable("work_item_comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content)
            .HasColumnName("content")
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne(c => c.WorkItem)
            .WithMany()
            .HasForeignKey("work_item_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey("user_read_model_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
