using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Management.Create;
using Modules.Projects.Domain.Entities;

namespace Modules.Projects.Application.Management.Patch;

public sealed record PatchProjectCommand(
    Guid ProjectId,
    string? Name,
    string? Abbreviation,
    ProjectPrivacy? PrivacyLevel)
    : ICommand<Result<ProjectResponse>>;
