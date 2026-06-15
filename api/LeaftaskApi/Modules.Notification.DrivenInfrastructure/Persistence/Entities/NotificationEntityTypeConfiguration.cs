using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Modules.Notification.DrivenInfrastructure.Persistence.Entities;

internal sealed class NotificationEntityTypeConfiguration
    : IEntityTypeConfiguration<Domain.Entities.Notification.Notification>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Notification.Notification> builder)
    {
        builder.ToTable("notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Type)
            .HasColumnName("type")
            .IsRequired();

        builder.Property(n => n.ContextId)
            .HasColumnName("context_id")
            .IsRequired();

        builder.Property(n => n.TargetId)
            .HasColumnName("target_id")
            .IsRequired();

        builder.Property(n => n.RecipientId)
            .HasColumnName("recipient_id")
            .IsRequired();

        builder.Property(n => n.ActorId)
            .HasColumnName("actor_id");

        builder.Property(n => n.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(n => n.ReadAt)
            .HasColumnName("read_at");
    }
}
