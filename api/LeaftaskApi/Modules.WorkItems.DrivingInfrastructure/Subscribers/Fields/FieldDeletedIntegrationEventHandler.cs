using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Projects.Integration;
using Modules.WorkItems.Application.Fields.DeleteFieldReadModelOnFieldDeleted;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivingInfrastructure.Subscribers.Fields;

public sealed class FieldDeletedIntegrationEventHandler(
    WorkItemsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<FieldDeletedIntegrationEvent, WorkItemsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        FieldDeletedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteFieldReadModelOnFieldDeletedCommand(notification.FieldId), cancellationToken);
    }

    protected override Guid GetFallbackMessageId(FieldDeletedIntegrationEvent notification) =>
        notification.FieldId;
}
