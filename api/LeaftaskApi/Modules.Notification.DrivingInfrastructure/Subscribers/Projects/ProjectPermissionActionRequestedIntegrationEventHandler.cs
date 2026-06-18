using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.ApprovalRequests.Create;
using Modules.Notification.Domain.Entities.Approval;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.Projects.Integration;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.Projects;

public sealed class ProjectPermissionActionRequestedIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<ProjectPermissionActionRequestedIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        ProjectPermissionActionRequestedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        Result<Guid> result = await sender.Send(new CreateApprovalRequestCommand(
            ContextType.Project,
            notification.ProjectId,
            notification.RequestedByUserId,
            notification.PermissionName,
            notification.ActionName,
            notification.ActionPayload), cancellationToken);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Failed to create project approval request: {result.Error.Code} — {result.Error.Description}");
    }

    protected override Guid GetFallbackMessageId(ProjectPermissionActionRequestedIntegrationEvent notification) =>
        notification.RequestedByUserId;
}
