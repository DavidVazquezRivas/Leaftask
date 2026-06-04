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
        builder.Property(a => a.Name).HasColumnName("name").IsRequired();
        builder.Property(a => a.Description).HasColumnName("description").IsRequired();
        builder.Property(a => a.SystemPrompt).HasColumnName("system_prompt").IsRequired();

        builder.HasOne(a => a.ModelConfig)
            .WithMany()
            .HasForeignKey("model_config_id")
            .IsRequired();
    }
}
