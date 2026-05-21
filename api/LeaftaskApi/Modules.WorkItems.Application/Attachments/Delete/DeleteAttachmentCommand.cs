using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.WorkItems.Application.Attachments.Delete;

public sealed record DeleteAttachmentCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Guid AttachmentId) : ICommand<Result>;
