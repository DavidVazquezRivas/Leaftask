using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Field;

namespace Modules.Projects.DrivenInfrastructure.Entities.Field;

internal sealed class OptionEntityTypeConfiguration : IEntityTypeConfiguration<Option>
{
    public void Configure(EntityTypeBuilder<Option> builder)
    {
        builder.ToTable("options");

        builder.HasKey(option => option.Id);

        builder.Property(option => option.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(option => option.Field)
            .WithMany()
            .HasForeignKey("field_id")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
