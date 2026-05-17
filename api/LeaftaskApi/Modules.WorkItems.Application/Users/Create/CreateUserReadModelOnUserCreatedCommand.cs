using BuildingBlocks.Application.Commands;

namespace Modules.WorkItems.Application.Users.Create;

public sealed record CreateUserReadModelOnUserCreatedCommand(
    Guid UserId,
    string FirstName,
    string LastName) : ICommand;
