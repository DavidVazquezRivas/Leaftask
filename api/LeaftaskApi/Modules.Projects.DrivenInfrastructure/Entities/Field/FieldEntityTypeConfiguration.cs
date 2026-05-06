using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Field;

namespace Modules.Projects.DrivenInfrastructure.Entities.Field;

internal sealed class FieldEntityTypeConfiguration : IEntityTypeConfiguration<Field>
{
    public void Configure(EntityTypeBuilder<Field> builder)
    {
        builder.ToTable("fields");

        builder.HasKey(field => field.Id);

        builder.Property(field => field.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(field => field.IsOptional)
            .HasColumnName("is_optional")
            .IsRequired();

        builder.HasOne(field => field.FieldType)
            .WithMany()
            .HasForeignKey("field_type_id")
            .OnDelete(DeleteBehavior.Restrict);
    }
}
