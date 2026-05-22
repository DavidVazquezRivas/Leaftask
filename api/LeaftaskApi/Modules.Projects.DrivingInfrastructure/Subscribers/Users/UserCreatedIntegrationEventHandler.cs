using BuildingBlocks.DrivingInfrastructure.Events;
using MediatR;
using Modules.Projects.Application.Users.Create;
using Modules.Projects.DrivenInfrastructure.Persistence;
using Modules.Users.Integration;

namespace Modules.Projects.DrivingInfrastructure.Subscribers.Users;

public sealed class UserCreatedIntegrationEventHandler(
    ProjectsDbContext dbContext,
    IIntegrationEventContextAccessor integrationEventContextAccessor,
    ISender sender)
    : IntegrationEventHandler<UserCreatedIntegrationEvent, ProjectsDbContext>(
        dbContext,
        integrationEventContextAccessor)
{
    protected override async Task HandleIntegrationEvent(UserCreatedIntegrationEvent notification,
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
