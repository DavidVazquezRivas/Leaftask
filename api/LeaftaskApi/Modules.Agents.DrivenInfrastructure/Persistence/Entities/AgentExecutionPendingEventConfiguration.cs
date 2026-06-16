using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Agents.Domain.Entities.Execution;

namespace Modules.Agents.DrivenInfrastructure.Persistence.Entities;

public sealed class AgentExecutionPendingEventConfiguration : IEntityTypeConfiguration<AgentExecutionPendingEvent>
{
    public void Configure(EntityTypeBuilder<AgentExecutionPendingEvent> builder)
    {
        builder.ToTable("agent_execution_pending_events");
        builder.HasKey(pe => pe.Id);
        builder.Property(pe => pe.Id).HasColumnName("id");
        builder.Property(pe => pe.ExecutionId).HasColumnName("execution_id");
        builder.Property(pe => pe.EventType).HasColumnName("event_type").IsRequired();
        builder.Property(pe => pe.CorrelationId).HasColumnName("correlation_id").IsRequired();
        builder.Property(pe => pe.IsResolved).HasColumnName("is_resolved");
    }
}
