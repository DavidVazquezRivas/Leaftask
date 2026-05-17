using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.WorkItems.Domain.Entities.Properties;

namespace Modules.WorkItems.DrivenInfrastructure.Entities;

internal sealed class WorkItemStatusEntityTypeConfiguration : IEntityTypeConfiguration<WorkItemStatus>
{
    public void Configure(EntityTypeBuilder<WorkItemStatus> builder)
    {
        builder.ToTable("work_item_statuses");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();
    }
}
