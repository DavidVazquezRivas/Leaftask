using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Notification.Domain.Entities.Approval;

namespace Modules.Notification.DrivenInfrastructure.Persistence.Entities;

internal sealed class RequestCommentEntityTypeConfiguration : IEntityTypeConfiguration<RequestComment>
{
    public void Configure(EntityTypeBuilder<RequestComment> builder)
    {
        builder.ToTable("request_comments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.RequestId)
            .HasColumnName("request_id")
            .IsRequired();

        builder.Property(c => c.Content)
            .HasColumnName("content")
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne(c => c.CreatedBy)
            .WithMany()
            .HasForeignKey("created_by_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
