using BuildingBlocks.Domain.Entities;

namespace Modules.WorkItems.Domain.Entities;

public sealed class Attachment : Entity
{
    private Attachment() { }

    public Attachment(Guid id, string fileName, Uri fileUrl, DateTime uploadedAt, WorkItem workItem, UserReadModel user)
    {
        Id = id;
        FileName = fileName;
        FileUrl = fileUrl;
        UploadedAt = uploadedAt;
        WorkItem = workItem;
        User = user;
    }

    public Guid Id { get; }
    public string FileName { get; } = string.Empty;
    public Uri FileUrl { get; } = null!;
    public DateTime UploadedAt { get; }
    public WorkItem WorkItem { get; } = null!;
    public UserReadModel User { get; } = null!;
    public WorkItemComment? Comment { get; private set; }

    public void LinkToComment(WorkItemComment? comment)
    {
        Comment = comment;
    }
}
