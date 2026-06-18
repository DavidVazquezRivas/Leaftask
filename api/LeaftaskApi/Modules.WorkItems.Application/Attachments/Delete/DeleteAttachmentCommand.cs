using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.WorkItems.Application.Attachments.Delete;

[RequireProjectPermission("work-items.edit-definition")]
public sealed record DeleteAttachmentCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Guid AttachmentId) : ICommand<Result>, IProjectPermissionRequest;
