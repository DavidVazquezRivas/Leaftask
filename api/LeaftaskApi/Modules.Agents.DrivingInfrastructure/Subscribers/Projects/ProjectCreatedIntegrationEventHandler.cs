using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Application.Projects.Create;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Modules.Projects.Integration;

namespace Modules.Agents.DrivingInfrastructure.Subscribers.Projects;

public sealed class ProjectCreatedIntegrationEventHandler(
    AgentsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<ProjectCreatedIntegrationEvent, AgentsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        ProjectCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new CreateProjectReadModelOnProjectCreatedCommand(notification.ProjectId, notification.Abbreviation),
            cancellationToken);
    }

    protected override Guid GetFallbackMessageId(ProjectCreatedIntegrationEvent notification) =>
        notification.ProjectId;
}
