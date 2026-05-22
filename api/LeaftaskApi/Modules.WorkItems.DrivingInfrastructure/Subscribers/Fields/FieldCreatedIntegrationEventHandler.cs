using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Projects.Integration;
using Modules.WorkItems.Application.Fields.CreateFieldReadModelOnFieldCreated;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivingInfrastructure.Subscribers.Fields;

public sealed class FieldCreatedIntegrationEventHandler(
    WorkItemsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<FieldCreatedIntegrationEvent, WorkItemsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        FieldCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(new CreateFieldReadModelOnFieldCreatedCommand(
            notification.FieldId,
            notification.Name,
            notification.IsOptional,
            notification.FieldTypeId,
            notification.WorkItemTypeIds), cancellationToken);
    }

    protected override Guid GetFallbackMessageId(FieldCreatedIntegrationEvent notification) =>
        notification.FieldId;
}
