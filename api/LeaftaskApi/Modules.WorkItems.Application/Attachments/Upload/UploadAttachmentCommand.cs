using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Application.Attachments.Upload;

public sealed record UploadAttachmentCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Stream FileContent,
    string FileName,
    string ContentType,
    long FileSize) : ICommand<Result<AttachmentDto>>;
