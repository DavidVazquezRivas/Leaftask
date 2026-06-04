using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Agents.Domain.Entities.AgentTriggers;

namespace Modules.Agents.DrivenInfrastructure.Persistence.Entities;

public sealed class AgentEventTriggerConfiguration : IEntityTypeConfiguration<AgentEventTrigger>
{
    public void Configure(EntityTypeBuilder<AgentEventTrigger> builder)
    {
        builder.ToTable("agent_event_triggers");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");
        builder.Property(t => t.UserPrompt).HasColumnName("user_prompt").IsRequired();
        builder.Property(t => t.Event).HasColumnName("event").IsRequired();

        builder.HasOne(t => t.Agent)
            .WithMany()
            .HasForeignKey("agent_id")
            .IsRequired();
    }
}
