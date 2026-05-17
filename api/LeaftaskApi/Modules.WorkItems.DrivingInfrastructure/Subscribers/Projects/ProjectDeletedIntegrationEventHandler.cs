using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Projects.Integration;
using Modules.WorkItems.Application.Projects.Delete;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivingInfrastructure.Subscribers.Projects;

public sealed class ProjectDeletedIntegrationEventHandler(
    WorkItemsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<ProjectDeletedIntegrationEvent, WorkItemsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        ProjectDeletedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteProjectReadModelOnProjectDeletedCommand(
            notification.ProjectId), cancellationToken);
    }

    protected override Guid GetFallbackMessageId(ProjectDeletedIntegrationEvent notification) =>
        notification.ProjectId;
}
