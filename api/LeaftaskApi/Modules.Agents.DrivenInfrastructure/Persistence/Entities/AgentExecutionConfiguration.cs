using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Agents.Domain.Entities.Execution;

namespace Modules.Agents.DrivenInfrastructure.Persistence.Entities;

public sealed class AgentExecutionConfiguration : IEntityTypeConfiguration<AgentExecution>
{
    public void Configure(EntityTypeBuilder<AgentExecution> builder)
    {
        builder.ToTable("agent_executions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.Payload).HasColumnName("payload").IsRequired();
        builder.Property(e => e.Status).HasColumnName("status");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");

        builder.HasOne(e => e.Agent)
            .WithMany()
            .HasForeignKey("agent_id")
            .IsRequired();

        builder.HasMany(e => e.PendingEvents)
            .WithOne()
            .HasForeignKey(pe => pe.ExecutionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
