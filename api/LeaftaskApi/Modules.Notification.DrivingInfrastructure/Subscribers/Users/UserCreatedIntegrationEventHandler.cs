using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Notification.Application.Users.Create;
using Modules.Notification.DrivenInfrastructure.Persistence;
using Modules.Users.Integration;

namespace Modules.Notification.DrivingInfrastructure.Subscribers.Users;

public sealed class UserCreatedIntegrationEventHandler(
    NotificationsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<UserCreatedIntegrationEvent, NotificationsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(
        UserCreatedIntegrationEvent notification,
        CancellationToken cancellationToken) =>
        await sender.Send(new CreateUserReadModelOnUserCreatedCommand(
            notification.UserId,
            notification.FirstName,
            notification.LastName), cancellationToken);

    protected override Guid GetFallbackMessageId(UserCreatedIntegrationEvent notification) =>
        notification.UserId;
}
