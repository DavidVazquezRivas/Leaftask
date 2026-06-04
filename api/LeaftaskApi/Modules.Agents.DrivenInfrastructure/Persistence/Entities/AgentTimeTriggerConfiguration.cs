using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Agents.Domain.Entities.AgentTriggers;

namespace Modules.Agents.DrivenInfrastructure.Persistence.Entities;

public sealed class AgentTimeTriggerConfiguration : IEntityTypeConfiguration<AgentTimeTrigger>
{
    public void Configure(EntityTypeBuilder<AgentTimeTrigger> builder)
    {
        builder.ToTable("agent_time_triggers");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");
        builder.Property(t => t.Name).HasColumnName("name").IsRequired();
        builder.Property(t => t.CronExpression).HasColumnName("cron_expression").IsRequired();
        builder.Property(t => t.TimeZone).HasColumnName("time_zone").IsRequired();
        builder.Property(t => t.IsActive).HasColumnName("is_active");

        builder.HasOne(t => t.Agent)
            .WithMany()
            .HasForeignKey("agent_id")
            .IsRequired();
    }
}
