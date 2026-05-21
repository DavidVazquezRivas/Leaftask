using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Application.Attachments.GetPresignedUrl;

public sealed record GetAttachmentPresignedUrlQuery(
    Guid ProjectId,
    Guid WorkItemId,
    string FileName) : IQuery<Result<PresignedUrlDto>>;
