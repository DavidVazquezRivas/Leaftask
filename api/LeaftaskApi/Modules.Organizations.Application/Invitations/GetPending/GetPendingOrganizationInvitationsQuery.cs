using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;

namespace Modules.Organizations.Application.Invitations.GetPending;

public sealed record GetPendingOrganizationInvitationsQuery(Guid OrganizationId)
    : IQuery<Result<IReadOnlyList<OrganizationInvitationResponse>>>;
