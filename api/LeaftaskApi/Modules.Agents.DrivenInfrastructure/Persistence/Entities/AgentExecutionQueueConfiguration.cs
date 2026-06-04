using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Agents.Domain.Entities.Queue;

namespace Modules.Agents.DrivenInfrastructure.Persistence.Entities;

public sealed class AgentExecutionQueueConfiguration : IEntityTypeConfiguration<AgentExecutionQueue>
{
    public void Configure(EntityTypeBuilder<AgentExecutionQueue> builder)
    {
        builder.ToTable("agent_execution_queues");
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).HasColumnName("id");
        builder.Property(q => q.Payload).HasColumnName("payload").IsRequired();
        builder.Property(q => q.Status).HasColumnName("status");
        builder.Property(q => q.CreatedAt).HasColumnName("created_at");
        builder.Property(q => q.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(q => q.Agent)
            .WithMany()
            .HasForeignKey("agent_id")
            .IsRequired();
    }
}
