using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.WorkItems.Application.Attachments.Upload;

[RequireProjectPermission("work-items.edit-definition")]
public sealed record UploadAttachmentCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Stream FileContent,
    string FileName,
    string ContentType,
    long FileSize) : ICommand<Result<AttachmentDto>>, IProjectPermissionRequest;
