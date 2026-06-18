using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.Projects.SyncMemberPermissions;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.Projects.Integration;
using SyncPermissionEntryDto = Modules.Notification.Application.Projects.SyncMemberPermissions.ProjectPermissionEntryDto;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.Projects;

public sealed class ProjectMemberJoinedIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<ProjectMemberJoinedIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        ProjectMemberJoinedIntegrationEvent notification,
        CancellationToken cancellationToken) =>
        await sender.Send(new SyncMemberProjectPermissionsCommand(
            notification.ProjectId,
            notification.UserId,
            notification.Permissions
                .Select(p => new SyncPermissionEntryDto(p.PermissionName, p.Level))
                .ToArray()), cancellationToken);

    protected override Guid GetFallbackMessageId(ProjectMemberJoinedIntegrationEvent notification) =>
        notification.UserId;
}
