using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.DrivenInfrastructure.Entities;

internal sealed class OrganizationRoleEntityTypeConfiguration : IEntityTypeConfiguration<OrganizationRole>
{
    public void Configure(EntityTypeBuilder<OrganizationRole> builder)
    {
        builder.ToTable("organization_roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(role => role.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Metadata
            .FindNavigation(nameof(OrganizationRole.Permissions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(role => role.Permissions)
            .WithOne()
            .HasForeignKey(rolePermission => rolePermission.OrganizationRoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(role => new { role.OrganizationId, role.Name })
            .IsUnique();
    }
}
