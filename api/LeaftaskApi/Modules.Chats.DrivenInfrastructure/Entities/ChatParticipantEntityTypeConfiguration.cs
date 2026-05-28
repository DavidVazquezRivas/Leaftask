using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Chats.Domain.Entities.Participant;

namespace Modules.Chats.DrivenInfrastructure.Entities;

internal sealed class ChatParticipantEntityTypeConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> builder)
    {
        builder.ToTable("chat_participants");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ParticipantId)
            .HasColumnName("participant_id")
            .IsRequired();

        builder.Property(p => p.ParticipantType)
            .HasColumnName("participant_type")
            .IsRequired();

        builder.Property(p => p.LastFetched)
            .HasColumnName("last_fetched")
            .IsRequired();

        builder.HasOne(p => p.Chat)
            .WithMany()
            .HasForeignKey("chat_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex("chat_id", nameof(ChatParticipant.ParticipantId)).IsUnique();
    }
}
