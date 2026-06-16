using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.ApprovalRequests.Create;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.Organizations.Integration;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.Organizations;

public sealed class OrganizationPermissionActionRequestedIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<OrganizationPermissionActionRequestedIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        OrganizationPermissionActionRequestedIntegrationEvent notification,
        CancellationToken cancellationToken) =>
        await sender.Send(new CreateApprovalRequestCommand(
            ContextType.Organization,
            notification.OrganizationId,
            notification.RequestedByUserId,
            notification.PermissionName), cancellationToken);

    protected override Guid GetFallbackMessageId(OrganizationPermissionActionRequestedIntegrationEvent notification) =>
        notification.RequestedByUserId;
}
