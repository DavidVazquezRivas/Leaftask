using BuildingBlocks.Application.Commands;

namespace Modules.Organizations.Application.Users.Create;

public sealed record CreateUserReadModelOnUserCreatedCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email) : ICommand;
