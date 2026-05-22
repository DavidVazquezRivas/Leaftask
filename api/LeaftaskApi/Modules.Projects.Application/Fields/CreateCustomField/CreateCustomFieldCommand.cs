using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Application.Fields.GetProjectCustomFields;

namespace Modules.Projects.Application.Fields.CreateCustomField;

[RequireProjectPermission("project.custom-fields")]
public sealed record CreateCustomFieldCommand(
    Guid ProjectId,
    string Name,
    Guid TypeId,
    IReadOnlyList<string> Options,
    bool Required,
    IReadOnlyList<Guid> AppliesTo)
    : ICommand<Result<CustomFieldDto>>, IProjectPermissionRequest;
