using BuildingBlocks.Application.Commands;

namespace Modules.Projects.Application.Users.Create;

public sealed record CreateUserReadModelOnUserCreatedCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email) : ICommand;
