using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApprovalAction = Modules.Notification.Domain.Entities.Approval.Action;

namespace Modules.Notification.DrivenInfrastructure.Persistence.Entities;

internal sealed class ActionEntityTypeConfiguration : IEntityTypeConfiguration<ApprovalAction>
{
    public void Configure(EntityTypeBuilder<ApprovalAction> builder)
    {
        builder.ToTable("actions");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Code)
            .HasColumnName("code")
            .HasMaxLength(100)
            .IsRequired();
    }
}
