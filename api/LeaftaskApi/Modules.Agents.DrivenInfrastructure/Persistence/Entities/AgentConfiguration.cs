using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Agents.Domain.Entities;

namespace Modules.Agents.DrivenInfrastructure.Persistence.Entities;

public sealed class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        builder.ToTable("agents");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");
        builder.Property(a => a.ProjectId).HasColumnName("project_id");
        builder.Property(a => a.Name).HasColumnName("name").IsRequired();
        builder.Property(a => a.Instructions).HasColumnName("instructions").IsRequired();
        builder.Property(a => a.SystemPrompt).HasColumnName("system_prompt").IsRequired();
        builder.Property(a => a.TemplateId).HasColumnName("template_id");
        builder.Property(a => a.CreatedAt).HasColumnName("created_at");

        builder.HasOne(a => a.ModelConfig)
            .WithMany()
            .HasForeignKey("model_config_id")
            .IsRequired();

        builder.HasOne<ProjectReadModel>()
            .WithMany()
            .HasForeignKey(a => a.ProjectId)
            .IsRequired();

        builder.HasMany(a => a.EventTriggers)
            .WithOne(t => t.Agent)
            .HasForeignKey("agent_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.TimeTriggers)
            .WithOne(t => t.Agent)
            .HasForeignKey("agent_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
