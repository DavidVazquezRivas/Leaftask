using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Notification.Domain.Entities.Approval;

namespace Modules.Notification.DrivenInfrastructure.Persistence.Entities;

internal sealed class ApprovalRequestEntityTypeConfiguration : IEntityTypeConfiguration<ApprovalRequest>
{
    public void Configure(EntityTypeBuilder<ApprovalRequest> builder)
    {
        builder.ToTable("approval_requests");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(a => a.ContextType)
            .HasColumnName("context_type")
            .IsRequired();

        builder.Property(a => a.ContextId)
            .HasColumnName("context_id")
            .IsRequired();

        builder.Property(a => a.TargetId)
            .HasColumnName("target_id")
            .IsRequired();

        builder.Property(a => a.PermissionName)
            .HasColumnName("permission_name")
            .IsRequired();

        builder.Property(a => a.ActionType)
            .HasColumnName("action_type");

        builder.Property(a => a.ActionPayload)
            .HasColumnName("action_payload");

        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasMany(a => a.Comments)
            .WithOne()
            .HasForeignKey(c => c.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Requester)
            .WithMany()
            .HasForeignKey("requester_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.ApproverRejecter)
            .WithMany()
            .HasForeignKey("approver_rejecter_id")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
