using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Management;

namespace Modules.Organizations.Application.Management.Patch;

public sealed record PatchOrganizationCommand(
    Guid Id,
    string? Name,
    string? Description,
    string? Website)
    : ICommand<Result<BasicOrganizationResponse>>;
