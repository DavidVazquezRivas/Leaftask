using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.WorkItems.Application.Attachments.GetPresignedUrl;

[RequireProjectPermission("Access Project")]
public sealed record GetAttachmentPresignedUrlQuery(
    Guid ProjectId,
    Guid WorkItemId,
    string FileName) : IQuery<Result<PresignedUrlDto>>, IProjectPermissionRequest;
