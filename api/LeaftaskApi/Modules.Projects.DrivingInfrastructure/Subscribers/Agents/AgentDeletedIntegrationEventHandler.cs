using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Agents.Integration;
using Modules.Projects.Application.Agents.Delete;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivingInfrastructure.Subscribers.Agents;

public sealed class AgentDeletedIntegrationEventHandler(
    ProjectsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<AgentDeletedIntegrationEvent, ProjectsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        AgentDeletedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new DeleteAgentReadModelOnAgentDeletedCommand(notification.AgentId, notification.ProjectId),
            cancellationToken);
    }

    protected override Guid GetFallbackMessageId(AgentDeletedIntegrationEvent notification) =>
        notification.AgentId;
}
