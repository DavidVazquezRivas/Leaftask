using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.Authorization;

namespace Modules.WorkItems.Application.Attachments.Delete;

[RequireProjectPermission("Access Project")]
public sealed record DeleteAttachmentCommand(
    Guid ProjectId,
    Guid WorkItemId,
    Guid AttachmentId) : ICommand<Result>, IProjectPermissionRequest;
