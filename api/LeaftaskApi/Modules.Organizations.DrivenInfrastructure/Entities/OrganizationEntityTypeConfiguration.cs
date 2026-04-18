using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.DrivenInfrastructure.Entities;

internal sealed class OrganizationEntityTypeConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organizations");


        builder.HasKey(o => o.Id);


        builder.Property(o => o.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        builder.Property(o => o.Website)
            .HasColumnName("website")
            .HasMaxLength(200);

        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();


        builder.Metadata
            .FindNavigation(nameof(Organization.Invitations))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Metadata
            .FindNavigation(nameof(Organization.Roles))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(o => o.Invitations)
            .WithOne()
            .HasForeignKey(invitation => invitation.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Roles)
            .WithOne()
            .HasForeignKey(role => role.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
