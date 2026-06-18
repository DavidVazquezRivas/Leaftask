using System.Text.Json;
using BuildingBlocks.Application.Authorization;
using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notifications.Integration;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivingInfrastructure.Subscribers.Notifications;

public sealed class ApprovalRequestResolvedIntegrationEventHandler(
    ProjectsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender,
    IProjectPermissionReplayContext replayContext)
    : IntegrationEventHandler<ApprovalRequestResolvedIntegrationEvent, ProjectsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        ApprovalRequestResolvedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        if (notification.Status != "Approved") return;
        if (string.IsNullOrEmpty(notification.ActionType) || string.IsNullOrEmpty(notification.ActionPayload)) return;

        Type? commandType = Type.GetType(notification.ActionType);
        if (commandType is null) return;

        object? command = JsonSerializer.Deserialize(notification.ActionPayload, commandType);
        if (command is null) return;

        replayContext.BeginReplay();
        await sender.Send(command, cancellationToken);
    }

    protected override Guid GetFallbackMessageId(ApprovalRequestResolvedIntegrationEvent notification) =>
        notification.RequestId;
}
