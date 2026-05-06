using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;

namespace Modules.Projects.Application.Management.Create;

[RequireOrganizationPermission("Create Projects")]
public sealed record CreateProjectCommand(
    string Name,
    string Abbreviation,
    Guid PrivacyLevelId,
    Guid? OrganizationId)
    : ICommand<Result<ProjectResponse>>, IOrganizationPermissionRequest;
