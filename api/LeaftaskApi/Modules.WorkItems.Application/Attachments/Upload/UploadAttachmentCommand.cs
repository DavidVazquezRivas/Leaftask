using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;

namespace Modules.WorkItems.Application.Attachments.Upload;

[RequireProjectPermission("Access Project")]
public sealed record UploadAttachmentCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Stream FileContent,
    string FileName,
    string ContentType,
    long FileSize) : ICommand<Result<AttachmentDto>>, IProjectPermissionRequest;
