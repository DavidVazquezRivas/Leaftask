using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Projects.Integration;
using Modules.WorkItems.Application.Fields.UpdateFieldReadModelOnFieldUpdated;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivingInfrastructure.Subscribers.Fields;

public sealed class FieldUpdatedIntegrationEventHandler(
    WorkItemsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<FieldUpdatedIntegrationEvent, WorkItemsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        FieldUpdatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateFieldReadModelOnFieldUpdatedCommand(
            notification.FieldId,
            notification.Name,
            notification.IsOptional,
            notification.FieldTypeId,
            notification.WorkItemTypeIds), cancellationToken);
    }

    protected override Guid GetFallbackMessageId(FieldUpdatedIntegrationEvent notification) =>
        notification.FieldId;
}
