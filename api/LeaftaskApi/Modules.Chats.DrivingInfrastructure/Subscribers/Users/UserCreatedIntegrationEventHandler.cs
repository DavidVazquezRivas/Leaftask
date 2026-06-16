using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Chats.Application.Users.Create;
using Modules.Chats.DrivenInfrastructure.Persistence;
using Modules.Users.Integration;

namespace Modules.Chats.DrivingInfrastructure.Subscribers.Users;

public sealed class UserCreatedIntegrationEventHandler(
    ChatsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<UserCreatedIntegrationEvent, ChatsDbContext>(
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
