using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Agents.Domain.Entities.Execution;

namespace Modules.Agents.DrivenInfrastructure.Persistence.Entities;

public sealed class AgentExecutionMessageConfiguration : IEntityTypeConfiguration<AgentExecutionMessage>
{
    public void Configure(EntityTypeBuilder<AgentExecutionMessage> builder)
    {
        builder.ToTable("agent_execution_messages");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id");
        builder.Property(m => m.ExecutionId).HasColumnName("execution_id");
        builder.Property(m => m.Role).HasColumnName("role");
        builder.Property(m => m.Content).HasColumnName("content").IsRequired();
        builder.Property(m => m.ToolCalls).HasColumnName("tool_calls");
        builder.Property(m => m.SequenceNumber).HasColumnName("sequence_number");

        builder.HasIndex(m => new { m.ExecutionId, m.SequenceNumber }).IsUnique();
    }
}
