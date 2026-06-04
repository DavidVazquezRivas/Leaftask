using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Application.Projects.Delete;
using Modules.Agents.DrivenInfrastructure.Persistence;
using Modules.Projects.Integration;

namespace Modules.Agents.DrivingInfrastructure.Subscribers.Projects;

public sealed class ProjectDeletedIntegrationEventHandler(
    AgentsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<ProjectDeletedIntegrationEvent, AgentsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        ProjectDeletedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new DeleteProjectReadModelOnProjectDeletedCommand(notification.ProjectId),
            cancellationToken);
    }

    protected override Guid GetFallbackMessageId(ProjectDeletedIntegrationEvent notification) =>
        notification.ProjectId;
}
