using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Members.Delete;

public sealed record DeleteOrganizationMemberCommand(
    Guid OrganizationId,
    Guid MemberId)
    : ICommand<Result>;
