using BuildingBlocks.Integration;

namespace Modules.Users.Integration;

public record UserCreatedIntegrationEvent(Guid UserId, string FirstName, string LastName, string Email)
    : IIntegrationEvent;
