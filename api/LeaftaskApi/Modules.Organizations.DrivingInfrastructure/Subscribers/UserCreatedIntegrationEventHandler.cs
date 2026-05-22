using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Organizations.Application.Users.Create;
using Modules.Organizations.DrivenInfrastructure.Persistence;
using Modules.Users.Integration;

namespace Modules.Organizations.DrivingInfrastructure.Subscribers;

public sealed class UserCreatedIntegrationEventHandler(
    OrganizationDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<UserCreatedIntegrationEvent, OrganizationDbContext>(
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
            notification.LastName,
            notification.Email), cancellationToken);
    }

    protected override Guid GetFallbackMessageId(UserCreatedIntegrationEvent notification) => notification.UserId;
}
