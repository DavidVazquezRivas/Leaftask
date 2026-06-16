using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Agents.Domain.Entities.Model;

namespace Modules.Agents.DrivenInfrastructure.Persistence.Entities;

public sealed class ModelConfigConfiguration : IEntityTypeConfiguration<ModelConfig>
{
    public void Configure(EntityTypeBuilder<ModelConfig> builder)
    {
        builder.ToTable("model_configs");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.Temperature).HasColumnName("temperature");
        builder.Property(c => c.MaxTokens).HasColumnName("max_tokens");

        builder.HasOne(c => c.Model)
            .WithMany()
            .HasForeignKey("model_id")
            .IsRequired();
    }
}
