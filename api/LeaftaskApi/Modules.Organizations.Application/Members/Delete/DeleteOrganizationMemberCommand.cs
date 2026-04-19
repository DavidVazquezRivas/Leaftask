using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Organizations.Application.Authorization;

namespace Modules.Organizations.Application.Members.Delete;

[RequireOrganizationPermission("Configure Organization")]
public sealed record DeleteOrganizationMemberCommand(
    Guid OrganizationId,
    Guid MemberId)
    : ICommand<Result>, IOrganizationPermissionRequest;
