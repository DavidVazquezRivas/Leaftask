using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Users.Integration;
using Modules.WorkItems.Application.Users.Create;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivingInfrastructure.Subscribers.Users;

public sealed class UserCreatedIntegrationEventHandler(
    WorkItemsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<UserCreatedIntegrationEvent, WorkItemsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        UserCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        await sender.Send(new CreateUserReadModelOnUserCreatedCommand(
            notification.UserId,
            notification.FirstName,
            notification.LastName), cancellationToken);
    }

    protected override Guid GetFallbackMessageId(UserCreatedIntegrationEvent notification) =>
        notification.UserId;
}
