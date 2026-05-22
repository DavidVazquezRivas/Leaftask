using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;

namespace Modules.WorkItems.Application.Attachments.GetPresignedUrl;

[RequireProjectPermission("Access Project")]
public sealed record GetAttachmentPresignedUrlQuery(
    Guid ProjectId,
    Guid WorkItemId,
    string FileName) : IQuery<Result<PresignedUrlDto>>, IProjectPermissionRequest;
