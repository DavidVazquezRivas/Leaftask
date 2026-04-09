using BuildingBlocks.Application.Events;
using BuildingBlocks.Domain.Events;
using BuildingBlocks.Integration;
using Modules.Users.Domain.Events;
using Modules.Users.Integration;

namespace Modules.Users.Application.Events;

public sealed class UserModuleEventMapper : IIntegrationEventMapper
{
    public IIntegrationEvent? Map(IDomainEvent domainEvent) =>
        domainEvent switch
        {
            UserCreatedDomainEvent e => new UserCreatedIntegrationEvent(e.UserId, e.FirstName, e.LastName, e.Email),


            _ => null
        };
}
