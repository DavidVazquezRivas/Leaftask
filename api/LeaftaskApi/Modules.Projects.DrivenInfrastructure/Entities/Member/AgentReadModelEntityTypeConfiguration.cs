using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Member;

namespace Modules.Projects.DrivenInfrastructure.Entities.Member;

internal sealed class AgentReadModelEntityTypeConfiguration : IEntityTypeConfiguration<AgentReadModel>
{
    public void Configure(EntityTypeBuilder<AgentReadModel> builder)
    {
        builder.ToTable("agent_read_models");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
    }
}
