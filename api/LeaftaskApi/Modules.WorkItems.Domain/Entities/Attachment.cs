using BuildingBlocks.Domain.Entities;

namespace Modules.WorkItems.Domain.Entities;

public sealed class Attachment : Entity
{
    private Attachment() { }

    public Attachment(Guid id, string fileName, Uri fileUrl, DateTime uploadedAt)
    {
        Id = id;
        FileName = fileName;
        FileUrl = fileUrl;
        UploadedAt = uploadedAt;
    }

    public Guid Id { get; }
    public string FileName { get; }
    public Uri FileUrl { get; }
    public DateTime UploadedAt { get; }
    public WorkItem WorkItem { get; }
    public UserReadModel User { get; }
    public WorkItemComment? Comment { get; }
}
