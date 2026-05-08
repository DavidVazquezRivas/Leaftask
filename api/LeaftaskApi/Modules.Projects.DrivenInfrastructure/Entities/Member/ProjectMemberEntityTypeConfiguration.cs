using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Projects.Domain.Entities.Member;

namespace Modules.Projects.DrivenInfrastructure.Entities.Member;

internal sealed class ProjectMemberEntityTypeConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("project_members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MemberId)
            .HasColumnName("member_id")
            .IsRequired();

        builder.Property(m => m.MemberType)
            .HasColumnName("member_type")
            .IsRequired();

        builder.Property(m => m.JoinedAt)
            .HasColumnName("joined_at")
            .IsRequired();

        builder.HasOne(m => m.Project)
            .WithMany()
            .HasForeignKey("project_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Role)
            .WithMany()
            .HasForeignKey("project_role_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex("project_id", nameof(ProjectMember.MemberId))
            .IsUnique();
    }
}
