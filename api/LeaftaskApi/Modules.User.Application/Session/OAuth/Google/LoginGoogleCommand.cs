using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;

namespace Modules.Users.Application.Session.OAuth.Google;

public record LoginGoogleCommand(string GoogleIdToken) : ICommand<Result<SessionResponse>>;
