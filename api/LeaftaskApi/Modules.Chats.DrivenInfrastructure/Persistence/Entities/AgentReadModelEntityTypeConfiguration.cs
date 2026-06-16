using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Chats.Domain.Entities.Participant;

namespace Modules.Chats.DrivenInfrastructure.Entities;

internal sealed class AgentReadModelEntityTypeConfiguration : IEntityTypeConfiguration<AgentReadModel>
{
    public void Configure(EntityTypeBuilder<AgentReadModel> builder)
    {
        builder.ToTable("agent_read_models");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
    }
}
