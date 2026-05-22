using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Organizations.Integration;
using Modules.Projects.Application.Organizations.Create;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivingInfrastructure.Subscribers.Organizations;

public sealed class OrganizationCreatedIntegrationEventHandler(
    ProjectsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<OrganizationCreatedIntegrationEvent, ProjectsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(OrganizationCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(new CreateOrganizationReadModelOnOrganizationCreatedCommand(
            notification.OrganizationId), cancellationToken);
    }

    protected override Guid GetFallbackMessageId(OrganizationCreatedIntegrationEvent notification) =>
        notification.OrganizationId;
}
