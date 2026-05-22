using BuildingBlocks.Domain.Events;

namespace Modules.Users.Domain.Events;

public record UserCreatedDomainEvent(Guid UserId, string FirstName, string LastName, string Email) : IDomainEvent;
