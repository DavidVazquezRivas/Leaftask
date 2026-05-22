using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.DrivenInfrastructure.Entities;

internal sealed class OrganizationInvitationEntityTypeConfiguration : IEntityTypeConfiguration<OrganizationInvitation>
{
    public void Configure(EntityTypeBuilder<OrganizationInvitation> builder)
    {
        builder.ToTable("organization_invitations");

        builder.HasKey(invitation => invitation.Id);

        builder.Property(invitation => invitation.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(invitation => invitation.InvitedAt)
            .HasColumnName("invited_at")
            .IsRequired();

        builder.Property(invitation => invitation.RespondedAt)
            .HasColumnName("responded_at");

        builder.Property(invitation => invitation.AbandonedAt)
            .HasColumnName("abandoned_at");

        builder.Property(invitation => invitation.OrganizationId)
            .HasColumnName("organization_id")
            .IsRequired();

        builder.Property(invitation => invitation.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(invitation => invitation.OrganizationRoleId)
            .HasColumnName("organization_role_id")
            .IsRequired();

        builder.HasIndex(invitation => new { invitation.OrganizationId, invitation.UserId })
            .IsUnique();

        builder.HasIndex(invitation => new { invitation.OrganizationId, invitation.Status });

        builder.HasOne<OrganizationRole>()
            .WithMany()
            .HasForeignKey(invitation => invitation.OrganizationRoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
