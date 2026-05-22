using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.WorkItems.Domain.Entities;

namespace Modules.WorkItems.DrivenInfrastructure.Entities;

internal sealed class AttachmentEntityTypeConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("attachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(a => a.FileUrl)
            .HasColumnName("file_url")
            .HasConversion(
                uri => uri.ToString(),
                str => new Uri(str))
            .IsRequired();

        builder.Property(a => a.UploadedAt)
            .HasColumnName("uploaded_at")
            .IsRequired();

        builder.HasOne(a => a.WorkItem)
            .WithMany()
            .HasForeignKey("work_item_id")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey("user_read_model_id")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Comment)
            .WithMany()
            .HasForeignKey("comment_id")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
