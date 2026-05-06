using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Field;

namespace Modules.Projects.DrivenInfrastructure.Entities.Field;

internal sealed class FieldTypeEntityTypeConfiguration : IEntityTypeConfiguration<FieldType>
{
    public void Configure(EntityTypeBuilder<FieldType> builder)
    {
        builder.ToTable("field_types");

        builder.HasKey(fieldType => fieldType.Id);

        builder.Property(fieldType => fieldType.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(fieldType => fieldType.Description)
            .HasColumnName("description")
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(fieldType => fieldType.Name)
            .IsUnique();
    }
}
