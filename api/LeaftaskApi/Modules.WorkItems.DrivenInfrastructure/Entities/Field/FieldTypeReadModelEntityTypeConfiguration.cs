using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.WorkItems.Domain.Entities.Field;

namespace Modules.WorkItems.DrivenInfrastructure.Entities.Field;

internal sealed class FieldTypeReadModelEntityTypeConfiguration : IEntityTypeConfiguration<FieldTypeReadModel>
{
    public void Configure(EntityTypeBuilder<FieldTypeReadModel> builder)
    {
        builder.ToTable("field_type_read_models");

        builder.HasKey(ft => ft.Id);

        builder.Property(ft => ft.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();
    }
}
