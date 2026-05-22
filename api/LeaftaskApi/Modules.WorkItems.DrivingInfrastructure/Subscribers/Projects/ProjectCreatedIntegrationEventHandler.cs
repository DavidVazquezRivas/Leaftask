using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Projects.Integration;
using Modules.WorkItems.Application.Projects.Create;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivingInfrastructure.Subscribers.Projects;

public sealed class ProjectCreatedIntegrationEventHandler(
    WorkItemsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<ProjectCreatedIntegrationEvent, WorkItemsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        ProjectCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(new CreateProjectReadModelOnProjectCreatedCommand(
            notification.ProjectId,
            notification.Abbreviation), cancellationToken);
    }

    protected override Guid GetFallbackMessageId(ProjectCreatedIntegrationEvent notification) =>
        notification.ProjectId;
}
