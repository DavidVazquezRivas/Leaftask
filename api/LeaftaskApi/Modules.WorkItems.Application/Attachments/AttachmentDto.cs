namespace Modules.WorkItems.Application.Attachments;

public sealed record AttachmentDto(Guid Id, string FileName, Uri Url);
