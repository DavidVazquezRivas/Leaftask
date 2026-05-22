using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Field;

namespace Modules.Projects.DrivenInfrastructure.Entities.Field;

internal sealed class WorkItemTypeReadModelEntityTypeConfiguration
    : IEntityTypeConfiguration<WorkItemTypeReadModel>
{
    public void Configure(EntityTypeBuilder<WorkItemTypeReadModel> builder)
    {
        builder.ToTable("workitem_type_read_models");

        builder.HasKey(wt => wt.Id);

        builder.Property(wt => wt.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);
    }
}
