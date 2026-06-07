using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Integration;
using Modules.Projects.Application.Agents.Create;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivingInfrastructure.Subscribers.Agents;

public sealed class AgentCreatedIntegrationEventHandler(
    ProjectsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<AgentCreatedIntegrationEvent, ProjectsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        AgentCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new CreateAgentReadModelOnAgentCreatedCommand(notification.AgentId, notification.Name, notification.ProjectId, notification.RoleId),
            cancellationToken);
    }

    protected override Guid GetFallbackMessageId(AgentCreatedIntegrationEvent notification) =>
        notification.AgentId;
}
