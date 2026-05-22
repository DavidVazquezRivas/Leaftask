using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Invitation;
using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.DrivenInfrastructure.Entities.Invitation;

internal sealed class ProjectInvitationEntityTypeConfiguration : IEntityTypeConfiguration<ProjectInvitation>
{
    public void Configure(EntityTypeBuilder<ProjectInvitation> builder)
    {
        builder.ToTable("project_invitations");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProjectId)
            .HasColumnName("project_id")
            .IsRequired();

        builder.Property(i => i.InviteeId)
            .HasColumnName("invitee_id")
            .IsRequired();

        builder.Property(i => i.InviteeType)
            .HasColumnName("invitee_type")
            .IsRequired();

        builder.Property(i => i.RoleId)
            .HasColumnName("project_role_id")
            .IsRequired();

        builder.Property(i => i.Status)
            .HasColumnName("status")
            .IsRequired();

        builder.Property(i => i.InvitedAt)
            .HasColumnName("invited_at")
            .IsRequired();

        builder.Property(i => i.RespondedAt)
            .HasColumnName("responded_at");

        builder.Property(i => i.CancelledAt)
            .HasColumnName("cancelled_at");

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ProjectRole>()
            .WithMany()
            .HasForeignKey(i => i.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new { i.ProjectId, i.InviteeId })
            .IsUnique();
    }
}
