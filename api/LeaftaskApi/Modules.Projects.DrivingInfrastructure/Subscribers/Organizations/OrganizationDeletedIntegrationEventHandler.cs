using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Organizations.Integration;
using Modules.Projects.Application.Organizations.Delete;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivingInfrastructure.Subscribers.Organizations;

public sealed class OrganizationDeletedIntegrationEventHandler(
    ProjectsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<OrganizationDeletedIntegrationEvent, ProjectsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(OrganizationDeletedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new DeleteOrganizationReadModelOnOrganizationDeletedCommand(notification.OrganizationId),
            cancellationToken);
    }

    protected override Guid GetFallbackMessageId(OrganizationDeletedIntegrationEvent notification) =>
        notification.OrganizationId;
}
