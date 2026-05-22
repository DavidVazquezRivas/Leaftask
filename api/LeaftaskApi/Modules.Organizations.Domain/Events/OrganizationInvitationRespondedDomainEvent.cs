using BuildingBlocks.Domain.Events;
using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Domain.Events;

public sealed record OrganizationInvitationRespondedDomainEvent(
    Guid OrganizationInvitationId,
    Guid OrganizationId,
    Guid UserId,
    Guid OrganizationRoleId,
    InvitationStatus Status) : IDomainEvent;
