using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.WorkItems.Domain.Entities.Properties;

namespace Modules.WorkItems.DrivenInfrastructure.Entities;

internal sealed class WorkItemTypeEntityTypeConfiguration : IEntityTypeConfiguration<WorkItemType>
{
    public void Configure(EntityTypeBuilder<WorkItemType> builder)
    {
        builder.ToTable("work_item_types");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();
    }
}
