namespace Modules.WorkItems.Application.Attachments;

public sealed record PresignedUrlDto(Uri PresignedUrl, Uri PublicUrl);
