using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.DrivenInfrastructure.Outbox;

internal sealed class OutboxMessageTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");


        builder.HasKey(x => x.Id);


        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Content)
            .HasColumnName("content")
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.OccurredAt)
            .HasColumnName("occurred_at")
            .IsRequired();

        builder.Property(x => x.ProcessedAt)
            .HasColumnName("processed_at");

        builder.Property(x => x.Error)
            .HasColumnName("error");


        // Index to optimize querying unprocessed messages
        builder.HasIndex(x => x.ProcessedAt)
            .HasFilter("\"processed_at\" IS NULL"); // Partial index
    }
}
