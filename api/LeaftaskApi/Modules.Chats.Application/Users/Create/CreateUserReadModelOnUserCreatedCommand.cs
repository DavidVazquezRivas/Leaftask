using BuildingBlocks.Application.Commands;

namespace Modules.Chats.Application.Users.Create;

public sealed record CreateUserReadModelOnUserCreatedCommand(
    Guid UserId,
    string FirstName,
    string LastName) : ICommand;
