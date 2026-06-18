using BuildingBlocks.Domain.Result;
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
        CancellationToken cancellationToken)
    {
        Result<Guid> result = await sender.Send(new CreateApprovalRequestCommand(
            ContextType.Organization,
            notification.OrganizationId,
            notification.RequestedByUserId,
            notification.PermissionName), cancellationToken);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Failed to create approval request: {result.Error.Code} — {result.Error.Description}");
    }

    protected override Guid GetFallbackMessageId(OrganizationPermissionActionRequestedIntegrationEvent notification) =>
        notification.RequestedByUserId;
}
