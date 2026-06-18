using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.Application.Authorization;

namespace Modules.Projects.Application.Fields.DeleteCustomField;

[RequireProjectPermission("project.custom-fields")]
public sealed record DeleteCustomFieldCommand(Guid ProjectId, Guid FieldId)
    : ICommand<Result>, IProjectPermissionRequest;
