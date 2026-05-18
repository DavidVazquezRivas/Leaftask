using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Field;
using FieldEntity = Modules.Projects.Domain.Entities.Field.Field;

namespace Modules.Projects.DrivenInfrastructure.Entities.Field;

internal sealed class FieldEntityTypeConfiguration : IEntityTypeConfiguration<FieldEntity>
{
    public void Configure(EntityTypeBuilder<FieldEntity> builder)
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

        builder.HasOne<FieldType>(field => field.FieldType)
            .WithMany()
            .HasForeignKey("field_type_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.AppliesTo)
            .WithMany()
            .UsingEntity(join => join.ToTable("field_workitem_type_read_models"));
    }
}
