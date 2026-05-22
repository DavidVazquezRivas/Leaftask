using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Authorization;
using Modules.Projects.Domain.Entities;

namespace Modules.Projects.Application.Management.Create;

[RequireOrganizationPermission("Create Projects")]
public sealed record CreateProjectCommand(
    string Name,
    string Abbreviation,
    ProjectPrivacy PrivacyLevel,
    Guid? OrganizationId)
    : ICommand<Result<ProjectResponse>>, IOrganizationPermissionRequest;
