using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.DrivenInfrastructure.Entities;

internal sealed class OrganizationPermissionEntityTypeConfiguration : IEntityTypeConfiguration<OrganizationPermission>
{
    public void Configure(EntityTypeBuilder<OrganizationPermission> builder)
    {
        builder.ToTable("organization_permissions");

        builder.HasKey(permission => permission.Id);

        builder.Property(permission => permission.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(permission => permission.Description)
            .HasColumnName("description")
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasIndex(permission => permission.Name)
            .IsUnique();
    }
}
